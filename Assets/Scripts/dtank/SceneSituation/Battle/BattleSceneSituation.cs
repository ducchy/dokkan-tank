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
        private readonly BattleResultData _resultData = new BattleResultData();
        
        private BattlePresenter _presenter;
        private DokkanTankRulePresenter _rulePresenter;

        protected override void ReleaseInternal(SituationContainer parent)
        {
            base.ReleaseInternal(parent);

            _stateContainer.Dispose();
            _rulePresenter?.Dispose();
        }

        protected override void StandbyInternal(Situation parent)
        {
            Debug.Log("BattleSceneSituation.StandbyInternal()");

            base.StandbyInternal(parent);

            ServiceContainer.Set(_stateContainer);
            ServiceContainer.Set(_resultData);
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
            SetupStateContainer();
            SetupPresenter();
        }

        private void SetupPresenter()
        {
            var camera = Services.Get<FollowTargetCamera>();

            var fieldView = Services.Get<FieldViewData>();
            var startPointDataArray = fieldView.StartPointDataArray;

            var controlUiView = Services.Get<BattleTankControlUiView>();
            controlUiView.Construct();

            var statusUiView = Services.Get<BattleTankStatusUiView>();
            statusUiView.Construct();

            var tankHolder = new GameObject("Tanks").transform;

            var tankModels = new List<BattleTankModel>();
            var tankActors = new List<BattleTankActor>();
            using (var actorFactory = new BattleTankActorFactory())
            {
                foreach (var startPointData in startPointDataArray)
                {
                    tankModels.Add(new BattleTankModel(startPointData));
                    var tankActor = actorFactory.Create(1, tankHolder);
                    tankActors.Add(tankActor);
                }
            }

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
                    playerTankPresenter.OnGameOver = () => _stateContainer.Change(BattleState.Result);
                    ServiceContainer.Set(playerTankPresenter);

                    _stateContainer.OnChangedState += playerTankPresenter.OnChangedState;
                    camera.Construct(tankActor.transform);
                    continue;
                }

                var npcTankBehaviourSelector =
                    new NpcBehaviourSelector(tankModel, tankModels.Where(m => m != tankModel).ToArray());
                var npcTankPresenter = new NpcBattleTankPresenter(tankController, tankModel, tankActor, npcTankBehaviourSelector);
                npcTankPresenters.Add(npcTankPresenter);
                _stateContainer.OnChangedState += npcTankPresenter.OnChangedState;
            }

            var controller = new BattleController(camera);
            ServiceContainer.Set(controller);

            _presenter = new BattlePresenter(controller, playerTankPresenter, npcTankPresenters.ToArray());
            
            _rulePresenter = new DokkanTankRulePresenter(_resultData, playerTankModel, tankModels.ToArray());
            _rulePresenter.OnGameEnd = () => _stateContainer.Change(BattleState.Result);
            _stateContainer.OnChangedState += _rulePresenter.OnChangedState;
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