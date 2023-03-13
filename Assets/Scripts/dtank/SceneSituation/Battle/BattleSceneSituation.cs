using System.Collections;
using System.Collections.Generic;
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

            var playerControlUiView = Services.Get<PlayerBattleTankControlUiView>();
            playerControlUiView.Construct();

            var tankHolder = new GameObject("Tanks").transform;

            PlayerBattleTankPresenter playerTankPresenter = null;
            var npcTankPresenters = new List<NpcBattleTankPresenter>();
            using (var actorFactory = new BattleTankActorFactory())
            {
                for (var i = 0; i < startPointDataArray.Length; i++)
                {
                    var startPointData = startPointDataArray[i];
                    var tankModel = new BattleTankModel();
                    var tankActor = actorFactory.Create(1, tankHolder);
                    tankActor.SetTransform(startPointData);

                    if (i == 0)
                    {
                        playerTankPresenter = new PlayerBattleTankPresenter(tankModel, tankActor, playerControlUiView);
                        camera.Construct(tankActor.transform);
                        continue;
                    }

                    npcTankPresenters.Add(new NpcBattleTankPresenter(tankModel, tankActor));
                }
            }

            if (playerTankPresenter != null)
            {
                ServiceContainer.Set(playerTankPresenter);
                _stateContainer.OnChangedState += playerTankPresenter.OnChangedState;
                playerTankPresenter.OnGameOver = () => _stateContainer.Change(BattleState.Result);
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