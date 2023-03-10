using GameFramework.Core;
using GameFramework.SituationSystems;
using GameFramework.StateSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dtank
{
	public class TitleSceneSituation : SceneSituation
	{
		protected override string SceneAssetPath => "Title";

		private readonly StateContainer<TitleStateBase, TitleState> _stateContainer = new StateContainer<TitleStateBase, TitleState>();
		private TitlePresenter _presenter;

		public TitleSceneSituation()
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
			Debug.Log("TitleSceneSituation.StandbyInternal()");

			base.StandbyInternal(parent);
			
			ServiceContainer.Set(_stateContainer);
		}

		protected override IEnumerator LoadRoutineInternal(TransitionHandle handle, IScope scope)
		{
			Debug.Log("Begin TitleSceneSituation.LoadRoutineInternal()");

			yield return base.LoadRoutineInternal(handle, scope);

			Debug.Log("End TitleSceneSituation.LoadRoutineInternal()");

			var uiView = Services.Get<TitleUiView>();
			uiView.Initialize();

			_presenter = new TitlePresenter(uiView);
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
	}
}
