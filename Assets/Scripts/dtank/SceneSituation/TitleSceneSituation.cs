using GameFramework.Core;
using GameFramework.SituationSystems;
using System.Collections;
using UnityEngine;

namespace dtank
{
	public class TitleSceneSituation : SceneSituation
	{
		protected override string SceneAssetPath => "Title";

		protected override IEnumerator LoadRoutineInternal(TransitionHandle handle, IScope scope)
		{
			Debug.Log("Begin TitleSceneSituation.LoadRoutineInternal()");

			yield return base.LoadRoutineInternal(handle, scope);

			Debug.Log("End TitleSceneSituation.LoadRoutineInternal()");
		}

		protected override void StandbyInternal(Situation parent)
		{
			Debug.Log("TitleSceneSituation.StandbyInternal()");

			base.StandbyInternal(parent);
		}
	}
}
