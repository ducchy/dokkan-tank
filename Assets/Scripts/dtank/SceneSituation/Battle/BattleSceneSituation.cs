using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Core;
using GameFramework.SituationSystems;
using GameFramework.StateSystems;
using GameFramework.TaskSystems;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace dtank
{
    public class BattleSceneSituation : SceneSituation
    {
        protected override string SceneAssetPath => "Battle";

        private StateContainer<BattleStateBase, BattleState> _stateContainer;
        private BattlePresenter _presenter;
        private List<ITask> _tasks = new List<ITask>();

        protected override void StandbyInternal(Situation parent)
        {
            Debug.Log("BattleSceneSituation.StandbyInternal()");

            base.StandbyInternal(parent);
        }

        protected override IEnumerator LoadRoutineInternal(TransitionHandle handle, IScope scope)
        {
            Debug.Log("Begin BattleSceneSituation.LoadRoutineInternal()");

            yield return base.LoadRoutineInternal(handle, scope);

            Debug.Log("End BattleSceneSituation.LoadRoutineInternal()");

            yield return LoadAll();

            SetupAll(scope);
        }

        protected override void UnloadInternal(TransitionHandle handle)
        {
            base.UnloadInternal(handle);

            UnloadAll();
        }

        protected override void ActivateInternal(TransitionHandle handle, IScope scope)
        {
            Debug.Log("BattleSceneSituation.ActivateInternal()");

            base.ActivateInternal(handle, scope);

            BattleModel.Get().ChangeState(BattleState.Ready);
        }

        protected override void UpdateInternal()
        {
            base.UpdateInternal();

            _stateContainer.Update(Time.deltaTime);
        }

        #region Load

        private IEnumerator LoadAll()
        {
            Debug.Log("Begin BattleSceneSituation.LoadAll()");

            yield return LoadField();

            Debug.Log("End BattleSceneSituation.LoadAll()");
        }

        private IEnumerator LoadField()
        {
            var fieldScene = new FieldScene(1);
            yield return fieldScene.LoadRoutine(ServiceContainer);
        }

        private void SetupAll(IScope scope)
        {
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

            var uiView = Services.Get<BattleUiView>();
            uiView.Setup();

            var controlUiView = Services.Get<BattleTankControlUiView>();
            controlUiView.Setup();
            RegisterTask(controlUiView, TaskOrder.UI);

            var statusUiView = Services.Get<BattleTankStatusUiView>();
            statusUiView.Setup();

            PlayerBattleTankPresenter playerTankPresenter = null;
            var npcTankPresenters = new List<NpcBattleTankPresenter>();
            BattleTankModel playerTankModel = null;

            var tankHolder = new GameObject("Tanks").transform;

            var tankActors = new List<BattleTankActor>();
            var tankActorDictionary = new Dictionary<int, BattleTankActor>();
            using (var actorFactory = new BattleTankActorFactory())
            {
                foreach (var tankModel in model.TankModelDictionary.Values)
                {
                    var tankActor = actorFactory.Create(tankModel.Data.ModelId, tankHolder);
                    tankActor.Construct(tankModel.Data.OwnerId);
                    tankActors.Add(tankActor);
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
                            new PlayerBattleTankPresenter(tankController, tankModel, tankActor, controlUiView,
                                statusUiView);
                        continue;
                    }

                    if (!model.BehaviourSelectorDictionary.TryGetValue(tankModel.Data.OwnerId,
                            out var npcTankBehaviourSelector))
                    {
                        Debug.LogError("CharacterType=NonPlayerのBehaviourSelectorが未生成");
                        continue;
                    }

                    var npcTankPresenter =
                        new NpcBattleTankPresenter(tankController, tankModel, tankActor, npcTankBehaviourSelector);
                    npcTankPresenters.Add(npcTankPresenter);
                }
            }

            var tankActorContainer = new TankActorContainer(tankActorDictionary);
            tankActorContainer.ScopeTo(scope);
            ServiceContainer.Set(tankActorContainer);

            var cameraController = Services.Get<BattleCameraController>();
            cameraController.Setup(playerTankModel, tankActorContainer);
            RegisterTask(cameraController, TaskOrder.Camera);

            var playingUiView = Services.Get<BattlePlayingUiView>();
            playingUiView.Setup();

            _presenter = new BattlePresenter(model, cameraController, playerTankPresenter, npcTankPresenters.ToArray(),
                tankActors.ToArray());
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
                            ParentContainer.Transition(new TitleSceneSituation());
                            break;
                        case BattleState.Retry:
                            Debug.LogError("BattleState.Retryは未実装");
                            break;
                        default:
                            _stateContainer.Change(state);
                            break;
                    }
                });
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
            UnloadField();

            var taskRunner = Services.Get<TaskRunner>();
            foreach (var task in _tasks)
                taskRunner.Unregister(task);

            BattleModel.Delete();
        }

        private void UnloadField()
        {
            SceneManager.UnloadSceneAsync("field001");
        }

        #endregion Unload
    }
}