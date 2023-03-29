using GameFramework.Core;
using GameFramework.SituationSystems;
using GameFramework.StateSystems;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace dtank
{
    public class TitleSceneSituation : SceneSituation
    {
        protected override string SceneAssetPath => "Title";

        private StateContainer<TitleStateBase, TitleState> _stateContainer;

        protected override IEnumerator LoadRoutineInternal(TransitionHandle handle, IScope scope)
        {
            Debug.Log("Begin TitleSceneSituation.LoadRoutineInternal()");

            yield return base.LoadRoutineInternal(handle, scope);

            yield return LoadAllRoutine(scope);

            _stateContainer.Change(TitleState.Idle);

            Debug.Log("End TitleSceneSituation.LoadRoutineInternal()");
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

        private void SetupModel(IScope scope)
        {
            var model = TitleModel.Create();
            model.ScopeTo(scope);
        }

        private void SetupPresenter(IScope scope)
        {
            var uiView = Services.Get<TitleUiView>();
            uiView.ScopeTo(scope);

            var camera = Services.Get<TitleCamera>();
            camera.Setup();
            camera.ScopeTo(scope);

            var model = TitleModel.Get();

            var presenter = new TitlePresenter(uiView, camera, model);
            presenter.ScopeTo(scope);
        }

        private void SetupStateContainer(IScope scope)
        {
            _stateContainer = new StateContainer<TitleStateBase, TitleState>();
            _stateContainer.ScopeTo(scope);

            var states = new List<TitleStateBase>()
            {
                new TitleStateIdle(),
                new TitleStateStart()
            };
            _stateContainer.Setup(TitleState.Invalid, states.ToArray());

            var model = TitleModel.Get();
            _stateContainer.OnChangedState += model.OnChangedState;
        }

        private void Bind(IScope scope)
        {
            var model = TitleModel.Get();
            model.CurrentState
                .TakeUntil(scope)
                .Subscribe(state =>
                {
                    switch (state)
                    {
                        case TitleState.End:
                            ParentContainer.Transition(new BattleReadySceneSituation(), new CommonFadeTransitionEffect(0.5f, 0f));
                            break;
                        default:
                            _stateContainer.Change(state);
                            break;
                    }
                })
                .ScopeTo(scope);
        }

        #endregion Load

        #region Unload

        private void UnloadAll()
        {
            var fieldManager = Services.Get<FieldManager>();
            fieldManager.Unload();

            TitleModel.Delete();
        }

        #endregion Unload
    }
}