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
        }

        #region Setup

        private void SetupAll()
        {
            SetupTanks();
            SetupStateContainer();
        }

        private void SetupTanks()
        {
            var fieldView = Services.Get<FieldViewData>();
            var startPointDataArray = fieldView.StartPointDataArray;

            var modelArray = startPointDataArray
                .Select(startPointData => new BattleTankModel(new TransformData(startPointData)))
                .ToArray();

            var viewList = new List<BattleTankView>();
            using (var viewFactory = new BattleTankViewFactory())
            {
                foreach (var model in modelArray)
                {
                    var view = viewFactory.Create(1);
                    view.SetTransform(model.TransformData.Value);
                    viewList.Add(view);
                }
            }

            var presenter = new BattleTankPresenter(modelArray, viewList.ToArray(), 0);
            ServiceContainer.Set(presenter);
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