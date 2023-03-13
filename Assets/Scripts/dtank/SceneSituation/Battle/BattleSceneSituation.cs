using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Core;
using GameFramework.SituationSystems;
using GameFramework.StateSystems;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace dtank
{
    public class BattleSceneSituation : SceneSituation
    {
        protected override string SceneAssetPath => "Battle";

        private readonly StateContainer<BattleStateBase, BattleState> _stateContainer =
            new StateContainer<BattleStateBase, BattleState>();

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

            for (var i = 0; i < tankModels.Count; i++)
            {
                var tankModel = tankModels[i];
                var tankActor = tankActors[i];
                var tankController = new BattleTankController(tankModel, tankActor);
                if (i == 0)
                {
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