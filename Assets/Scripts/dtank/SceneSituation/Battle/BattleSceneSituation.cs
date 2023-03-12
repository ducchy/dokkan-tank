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
            _presenter?.Update();
        }

        #region Setup

        private void SetupAll()
        {
            SetupStateContainer();
            SetupPresenter();
        }

        private void SetupPresenter()
        {
            var fieldView = Services.Get<FieldViewData>();
            var startPointDataArray = fieldView.StartPointDataArray;

            var modelArray = startPointDataArray
                .Select(startPointData => new BattleTankModel(new TransformData(startPointData)))
                .ToArray();

            var actorList = new List<BattleTankActor>();
            using (var actorFactory = new BattleTankActorFactory())
            {
                foreach (var model in modelArray)
                {
                    var actor = actorFactory.Create(1);
                    actor.SetTransform(model.TransformData.Value);
                    actorList.Add(actor);
                }
            }

            var tankPresenter = new BattleTankPresenter(modelArray, actorList.ToArray());
            
            var camera = Services.Get<FollowTargetCamera>();
            camera.Construct(actorList[0].transform);

            var controller = new BattleController(camera);
            _presenter = new BattlePresenter(controller, tankPresenter);
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