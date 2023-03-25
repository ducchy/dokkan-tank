using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Core;
using GameFramework.CoroutineSystems;
using GameFramework.SituationSystems;
using GameFramework.StateSystems;
using GameFramework.TaskSystems;
using UniRx;
using UnityEngine;

namespace dtank
{
    public class BattleSceneSituation : SceneSituation
    {
        protected override string SceneAssetPath => "Battle";

        private StateContainer<BattleStateBase, BattleState> _stateContainer;
        private BattlePresenter _presenter;
        private readonly List<ITask> _tasks = new List<ITask>();

        protected override IEnumerator LoadRoutineInternal(TransitionHandle handle, IScope scope)
        {
            Debug.Log("Begin BattleSceneSituation.LoadRoutineInternal()");

            yield return base.LoadRoutineInternal(handle, scope);

            var fieldManager = Services.Get<FieldManager>();
            yield return fieldManager.LoadRoutine(1);

            Debug.Log("End BattleSceneSituation.LoadRoutineInternal()");
        }

        protected override IEnumerator SetupRoutineInternal(TransitionHandle handle, IScope scope)
        {
            yield return base.SetupRoutineInternal(handle, scope);

            yield return SetupAllRoutine(scope);
            
            Bind(scope);

            BattleModel.Get().ChangeState(BattleState.Ready);
        }

        protected override void UpdateInternal()
        {
            base.UpdateInternal();

            _stateContainer.Update(Time.deltaTime);
        }

        protected override void UnloadInternal(TransitionHandle handle)
        {
            base.UnloadInternal(handle);

            UnloadAll();
        }

        #region Load

        private IEnumerator SetupAllRoutine(IScope scope)
        {
            SetupBattleEntryData();
            yield return SetupModelRoutine(scope);
            SetupPresenter(scope);
            SetupStateContainer(scope);
        }

        private void SetupBattleEntryData()
        {
            // TODO: 選んだルールに応じて設定
            var battleEntryData = Services.Get<BattleEntryData>();
            battleEntryData.Set(1, new List<BattlePlayerEntryData>()
            {
                new("一郎", 1, 0, CharacterType.Player, 1),
                new("二郎", 2, 1, CharacterType.NonPlayer, 1),
                new("三郎", 3, 2, CharacterType.NonPlayer, 1),
                new("四郎", 4, 3, CharacterType.NonPlayer, 1),
            });
        }

        private IEnumerator SetupModelRoutine(IScope scope)
        {
            var battleEntryData = Services.Get<BattleEntryData>();
            var fieldViewData = Services.Get<FieldViewData>();
            
            var battleModel = BattleModel.Create();
            RegisterTask(battleModel, TaskOrder.Logic);
            yield return battleModel.SetupAsync(battleEntryData, fieldViewData)
                .StartAsEnumerator(scope);
            battleModel.ScopeTo(scope);
        }

        private void SetupPresenter(IScope scope)
        {
            var model = BattleModel.Get();

            var fadeController = Services.Get<FadeController>();
            var uiView = Services.Get<BattleUiView>();
            uiView.Setup(fadeController);
            uiView.ScopeTo(scope);
            RegisterTask(uiView, TaskOrder.UI);

            PlayerBattleTankPresenter playerTankPresenter = null;
            var npcTankPresenters = new List<NpcBattleTankPresenter>();
            BattleTankModel playerTankModel = null;

            var tankHolder = new GameObject("Tanks").transform;

            var tankActorDictionary = new Dictionary<int, BattleTankActor>();
            using (var actorFactory = new BattleTankActorFactory())
            {
                foreach (var tankModel in model.TankModels)
                {
                    var tankActor = actorFactory.Create(tankModel.ModelId, tankHolder);
                    tankActor.Setup(tankModel.Id);
                    tankActorDictionary.Add(tankModel.Id, tankActor);

                    var tankController = new BattleTankController(tankModel, tankActor);
                    if (tankModel.CharacterType == CharacterType.Player)
                    {
                        if (playerTankModel != null)
                        {
                            Debug.LogError("CharacterType=Playerのタンクが複数存在");
                            continue;
                        }

                        playerTankModel = tankModel;
                        playerTankPresenter =
                            new PlayerBattleTankPresenter(tankController, model, tankModel, tankActor,
                                uiView.TankControlUiView,
                                uiView.TankStatusUiView);
                        continue;
                    }

                    if (!model.BehaviourSelectorDictionary.TryGetValue(tankModel.Id,
                            out var npcTankBehaviourSelector))
                    {
                        Debug.LogError("CharacterType=NonPlayerのBehaviourSelectorが未生成");
                        continue;
                    }

                    var npcTankPresenter =
                        new NpcBattleTankPresenter(tankController, model, tankModel, tankActor,
                            npcTankBehaviourSelector);
                    npcTankPresenters.Add(npcTankPresenter);
                }
            }

            var tankActorContainer = new TankActorContainer(tankActorDictionary);
            tankActorContainer.ScopeTo(scope);

            var cameraController = Services.Get<BattleCameraController>();
            cameraController.Setup(playerTankModel, tankActorContainer);
            cameraController.ScopeTo(scope);
            RegisterTask(cameraController, TaskOrder.Camera);

            _presenter = new BattlePresenter(model, uiView, cameraController, playerTankPresenter,
                npcTankPresenters.ToArray(),
                tankActorContainer);
            _presenter.ScopeTo(scope);
        }

        private void SetupStateContainer(IScope scope)
        {
            Debug.Log("BattleSceneSituation.SetupStateContainer");

            _stateContainer = new StateContainer<BattleStateBase, BattleState>();
            _stateContainer.ScopeTo(scope);

            var states = new List<BattleStateBase>()
            {
                new BattleStateReady(),
                new BattleStatePlaying(),
                new BattleStateResult(),
            };
            _stateContainer.Setup(BattleState.Invalid, states.ToArray());
        }

        private void Bind(IScope scope)
        {
            var battleModel = BattleModel.Get();
            battleModel.CurrentState
                .TakeUntil(scope)
                .Subscribe(state =>
                {
                    switch (state)
                    {
                        case BattleState.Quit:
                            ParentContainer.Transition(new TitleSceneSituation(), new CommonFadeTransitionEffect());
                            break;
                        case BattleState.Retry:
                            Debug.LogError("BattleState.Retryは未実装");
                            break;
                        default:
                            _stateContainer.Change(state);
                            break;
                    }
                })
                .ScopeTo(scope);
        }

        private void RegisterTask(ITask task, TaskOrder order)
        {
            var taskRunner = Services.Get<TaskRunner>();
            taskRunner.Register(task, order);
            _tasks.Add(task);
        }

        #endregion Load

        #region Unload

        private void UnloadAll()
        {
            var fieldManager = Services.Get<FieldManager>();
            fieldManager.Unload();

            var taskRunner = Services.Get<TaskRunner>();
            foreach (var task in _tasks)
                taskRunner.Unregister(task);

            BattleModel.Delete();
        }

        #endregion Unload
    }
}