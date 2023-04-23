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

        protected override IEnumerator SetupRoutineInternal(TransitionHandle handle, IScope scope)
        {
            Debug.Log("[TitleSceneSituation] Begin SetupRoutineInternal");

            yield return SetupAllRoutine(scope);

            Bind(scope);

            TitleModel.Get().ChangeState(TitleState.Enter);

            Debug.Log("[TitleSceneSituation] End SetupRoutineInternal");
        }

        protected override void UpdateInternal()
        {
            _stateContainer.Update(Time.deltaTime);
        }

        protected override void UnloadInternal(TransitionHandle handle)
        {
            Debug.Log("[TitleSceneSituation] UnloadInternal");

            UnloadAll();
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
                        case TitleState.ToBattle:
                            ParentContainer.Transition(new BattleReadySceneSituation(),
                                new CommonFadeTransitionEffect(true, false));
                            break;
                        default:
                            _stateContainer.Change(state);
                            break;
                    }
                })
                .ScopeTo(scope);
        }

        #region Setup

        private IEnumerator SetupAllRoutine(IScope scope)
        {
            var fieldManager = Services.Get<FieldManager>();
            yield return fieldManager.LoadRoutine(1);

            var model = TitleModel.Create();
            model.ScopeTo(scope);

            var uiView = Services.Get<TitleUiView>();
            uiView.Setup();

            SetupStateContainer(scope);
        }

        private void SetupStateContainer(IScope scope)
        {
            _stateContainer = new StateContainer<TitleStateBase, TitleState>();
            _stateContainer.ScopeTo(scope);

            var states = new List<TitleStateBase>()
            {
                new TitleStateEnter(),
                new TitleStateIdle(),
                new TitleStateExit(),
            };
            _stateContainer.Setup(TitleState.Invalid, states.ToArray());
        }

        #endregion Setup

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