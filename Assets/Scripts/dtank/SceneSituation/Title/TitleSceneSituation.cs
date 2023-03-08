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

		private StateContainer<TitleStateBase, TitleState> _stateContainer = new StateContainer<TitleStateBase, TitleState>();

		public TitleSceneSituation()
		{
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
		}

		protected override void StandbyInternal(Situation parent)
		{
			Debug.Log("TitleSceneSituation.StandbyInternal()");

			base.StandbyInternal(parent);
		}

		protected override IEnumerator LoadRoutineInternal(TransitionHandle handle, IScope scope)
		{
			Debug.Log("Begin TitleSceneSituation.LoadRoutineInternal()");

			yield return base.LoadRoutineInternal(handle, scope);

			Debug.Log("End TitleSceneSituation.LoadRoutineInternal()");
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
