using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Core;
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

            yield return LoadAllRoutine(scope);

            BattleModel.Get().ChangeState(BattleState.Ready);

            Debug.Log("End BattleSceneSituation.LoadRoutineInternal()");
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

        private IEnumerator LoadAllRoutine(IScope scope)
        {
            var fieldManager = Services.Get<FieldManager>();
            yield return fieldManager.LoadRoutine(1);
            
            SetupModel(scope);
            SetupPresenter(scope);
            SetupStateContainer(scope);

            Bind(scope);
        }

        private List<TankData> CreateTankDataList()
        {
            return new List<TankData>()
            {
                new(1, 1, 0, CharacterType.Player, 2f),
                new(2, 2, 1, CharacterType.NonPlayer, 2f),
                new(3, 3, 2, CharacterType.NonPlayer, 2f),
                new(4, 4, 3, CharacterType.NonPlayer, 2f),
            };
        }

        private void SetupModel(IScope scope)
        {
            var tankDataList = CreateTankDataList();

            var fieldViewData = Services.Get<FieldViewData>();
            var tankModels = new List<BattleTankModel>();
            var tankModelDictionary = new Dictionary<int, BattleTankModel>();
            foreach (var tankData in tankDataList)
            {
                var startPointData = fieldViewData.StartPointDataArray[tankData.PositionIndex];

                var tankModel = new BattleTankModel(tankData, startPointData);
                tankModels.Add(tankModel);
                tankModelDictionary.Add(tankData.OwnerId, tankModel);
            }

            var behaviourSelectorDictionary = new Dictionary<int, IBehaviourSelector>();
            foreach (var tankModel in tankModels)
            {
                if (tankModel.Data.CharacterType == CharacterType.Player)
                    continue;

                var npcBehaviourSelector =
                    new NpcBehaviourSelector(tankModel, tankModels.Where(m => m != tankModel).ToArray());
                behaviourSelectorDictionary.Add(tankModel.Data.OwnerId, npcBehaviourSelector);
            }

            var ruleModel = new BattleRuleModel(90f);
            ruleModel.ScopeTo(scope);

            var battleModel = BattleModel.Create();
            battleModel.Setup(ruleModel, tankModelDictionary, behaviourSelectorDictionary);
            battleModel.ScopeTo(scope);

            RegisterTask(battleModel, TaskOrder.Logic);
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
                foreach (var tankModel in model.TankModelDictionary.Values)
                {
                    var tankActor = actorFactory.Create(tankModel.Data.ModelId, tankHolder);
                    tankActor.Setup(tankModel.Data.OwnerId);
                    tankActorDictionary.Add(tankModel.Data.OwnerId, tankActor);

                    var tankController = new BattleTankController(tankModel, tankActor);
                    if (tankModel.Data.CharacterType == CharacterType.Player)
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

                    if (!model.BehaviourSelectorDictionary.TryGetValue(tankModel.Data.OwnerId,
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

            _presenter = new BattlePresenter(model, uiView, cameraController, playerTankPresenter, npcTankPresenters.ToArray(),
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