using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Core;
using GameFramework.SituationSystems;
using GameFramework.StateSystems;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace dtank
{
    public class BattleSceneSituation : SceneSituation
    {
        protected override string SceneAssetPath => "Battle";

        private readonly StateContainer<BattleStateBase, BattleState> _stateContainer =
            new StateContainer<BattleStateBase, BattleState>();
        private readonly BattleRuleModel _ruleModel = new BattleRuleModel(30f);
        
        private BattlePresenter _presenter;

        protected override void ReleaseInternal(SituationContainer parent)
        {
            base.ReleaseInternal(parent);

            _stateContainer.Dispose();
        }

        protected override void StandbyInternal(Situation parent)
        {
            Debug.Log("BattleSceneSituation.StandbyInternal()");

            base.StandbyInternal(parent);

            ServiceContainer.Set(_stateContainer);
            ServiceContainer.Set(_ruleModel);
        }

        protected override IEnumerator LoadRoutineInternal(TransitionHandle handle, IScope scope)
        {
            Debug.Log("Begin BattleSceneSituation.LoadRoutineInternal()");

            yield return base.LoadRoutineInternal(handle, scope);

            Debug.Log("End TitleSceneSituation.LoadRoutineInternal()");

            yield return LoadAll();

            SetupAll();
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

            _stateContainer.Change(BattleState.Ready);
        }

        protected override void UpdateInternal()
        {
            base.UpdateInternal();

            _stateContainer.Update(Time.deltaTime);
            _presenter?.Update(Time.deltaTime);
        }

        #region Setup

        private void SetupAll()
        {
            SetupPresenter();
            SetupStateContainer();
        }

        private void SetupPresenter()
        {
            var uiView = Services.Get<BattleUiView>();
            uiView.Construct();
            
            var camera = Services.Get<BattleCamera>();
            camera.Construct();

            var fieldView = Services.Get<FieldViewData>();
            var startPointDataArray = fieldView.StartPointDataArray;

            var controlUiView = Services.Get<BattleTankControlUiView>();
            controlUiView.Construct();

            var statusUiView = Services.Get<BattleTankStatusUiView>();
            statusUiView.Construct();

            var tankHolder = new GameObject("Tanks").transform;

            var tankModels = new List<BattleTankModel>();
            var tankActors = new List<BattleTankActor>();
            var tankActorDictionary = new Dictionary<int, BattleTankActor>();
            using (var actorFactory = new BattleTankActorFactory())
            {
                var tankId = 1;
                foreach (var startPointData in startPointDataArray)
                {
                    int playerId = tankId;
                    tankModels.Add(new BattleTankModel(playerId, startPointData, 2f));
                    var tankActor = actorFactory.Create(tankId, tankHolder);
                    tankActor.Construct(playerId);
                    tankActors.Add(tankActor);
                    tankActorDictionary.Add(tankId++, tankActor);
                }
            }
            var tankActorContainer = new TankActorContainer(tankActorDictionary);
            ServiceContainer.Set(tankActorContainer);

            PlayerBattleTankPresenter playerTankPresenter = null;
            var npcTankPresenters = new List<NpcBattleTankPresenter>();
            BattleTankModel playerTankModel = null;

            for (var i = 0; i < tankModels.Count; i++)
            {
                var tankModel = tankModels[i];
                var tankActor = tankActors[i];
                var tankController = new BattleTankController(tankModel, tankActor);
                if (i == 0)
                {
                    playerTankModel = tankModel;
                    playerTankPresenter =
                        new PlayerBattleTankPresenter(tankController, tankModel, tankActor, controlUiView, statusUiView);
                    ServiceContainer.Set(playerTankPresenter);
                    continue;
                }

                var npcTankBehaviourSelector =
                    new NpcBehaviourSelector(tankModel, tankModels.Where(m => m != tankModel).ToArray());
                var npcTankPresenter = new NpcBattleTankPresenter(tankController, tankModel, tankActor, npcTankBehaviourSelector);
                npcTankPresenters.Add(npcTankPresenter);
            }

            var controller = new BattleController(camera, playerTankModel, tankActorContainer);
            ServiceContainer.Set(controller);

            var playingUiView = Services.Get<BattlePlayingUiView>();
            
            var rulePresenter = new DokkanTankRulePresenter(_ruleModel, tankActors.ToArray(), tankModels.ToArray(), playingUiView);

            _presenter = new BattlePresenter(controller, playerTankPresenter, npcTankPresenters.ToArray(), rulePresenter);
        }

        private void SetupStateContainer()
        {
            var states = new List<BattleStateBase>()
            {
                new BattleStateReady(),
                new BattleStatePlaying(),
                new BattleStateResult()
            };
            _stateContainer.Setup(BattleState.Invalid, states.ToArray());
            _stateContainer.OnChangedState += _presenter.OnChangedState;
        }

        #endregion Setup

        #region Load

        private IEnumerator LoadAll()
        {
            yield return LoadField();
        }

        private IEnumerator LoadField()
        {
            var fieldScene = new FieldScene(1);
            yield return fieldScene.LoadRoutine(ServiceContainer);
        }

        private void UnloadAll()
        {
            UnloadField();
        }

        private void UnloadField()
        {
            SceneManager.UnloadSceneAsync("field001");
        }

        #endregion Laod
    }
}