using System.Collections;
using System.Collections.Generic;
using GameFramework.Core;
using GameFramework.SituationSystems;
using GameFramework.StateSystems;
using UnityEngine;

namespace dtank
{	
    public class BattleSceneSituation : SceneSituation
    {
        protected override string SceneAssetPath => "Battle";

        private readonly StateContainer<TitleStateBase, TitleState> _stateContainer = new StateContainer<TitleStateBase, TitleState>();
        private BattlePresenter _presenter;

        public BattleSceneSituation()
        {
            SetupStateContainer();
        }

        protected override void ReleaseInternal(SituationContainer parent)
        {
            base.ReleaseInternal(parent);

            _stateContainer.Dispose();
            _presenter?.Dispose();
        }

        private void SetupStateContainer()
        {
            var states = new List<TitleStateBase>()
            {
                new TitleStateIdle(),
                new TitleStateStart()
            };
            _stateContainer.Setup(TitleState.Invalid, states.ToArray());
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

            var titleUiView = Services.Get<TitleUiView>();
            titleUiView.Initialize();

            _presenter = new BattlePresenter();
        }

        protected override void ActivateInternal(TransitionHandle handle, IScope scope)
        {
            Debug.Log("BattleSceneSituation.ActivateInternal()");

            base.ActivateInternal(handle, scope);

            _stateContainer.Change(TitleState.Idle);
        }

        protected override void UpdateInternal()
        {
            base.UpdateInternal();

            _stateContainer.Update(Time.deltaTime);
        }
    }

}