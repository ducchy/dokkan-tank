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

		private readonly SceneSituationContainer _sceneSituationContainer = null;
		private readonly StateContainer<TitleStateBase, TitleState> _stateContainer = new StateContainer<TitleStateBase, TitleState>();
		private TitlePresenter _presenter = null;

		public TitleSceneSituation()
		{
			_sceneSituationContainer = Services.Get<SceneSituationContainer>();
		}

		protected override void ReleaseInternal(SituationContainer parent)
		{
			base.ReleaseInternal(parent);

			_stateContainer.OnChangedState -= _presenter.OnChangeState;
			_stateContainer.Dispose();
			_presenter?.Dispose();
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

			yield return LoadField();
			
			SetupAll();
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

		private void SetupAll()
		{
			SetupPresenter();
			SetupStateContainer();
		}

		private void SetupStateContainer()
		{
			var states = new List<TitleStateBase>()
			{
				new TitleStateIdle(),
				new TitleStateStart()
			};
			_stateContainer.Setup(TitleState.Invalid, states.ToArray());
			_stateContainer.OnChangedState += _presenter.OnChangeState;
		}
		
		private void SetupPresenter()
		{
			var uiView = Services.Get<TitleUiView>();
			uiView.Construct();

			var camera = Services.Get<TitleCamera>();
			camera.Construct();

			_presenter = new TitlePresenter(uiView, camera);
			_presenter.OnTouchToStart = () => _stateContainer.Change(TitleState.Start);
			_presenter.OnEndTitle = () => _sceneSituationContainer.Transition(new BattleSceneSituation());
		}
		
		#endregion Setup
	}
}
