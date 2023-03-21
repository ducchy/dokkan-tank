using GameFramework.Core;
using GameFramework.SituationSystems;
using GameFramework.StateSystems;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

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

            Debug.Log("End TitleSceneSituation.LoadRoutineInternal()");

            yield return LoadField();

            SetupAll(scope);
        }

        protected override void ActivateInternal(TransitionHandle handle, IScope scope)
        {
            Debug.Log("TitleSceneSituation.ActivateInternal()");

            base.ActivateInternal(handle, scope);

            _stateContainer.Change(TitleState.Idle);
        }

        protected override void UpdateInternal()
        {
            base.UpdateInternal();
            
            _stateContainer.Update(Time.deltaTime);
        }
        
        private IEnumerator LoadField()
        {
            var fieldScene = new FieldScene(1);
            yield return fieldScene.LoadRoutine(ServiceContainer);
        }

        #region Setup

        private void SetupAll(IScope scope)
        {
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

            var camera = Services.Get<TitleCamera>();
            camera.Setup();

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
                            ParentContainer.Transition(new BattleSceneSituation(), new CommonFadeTransitionEffect());
                            break;
                        default: 
                            _stateContainer.Change(state);
                            break;
                    }
                })
                .ScopeTo(scope);
        }

        #endregion Setup
    }
}