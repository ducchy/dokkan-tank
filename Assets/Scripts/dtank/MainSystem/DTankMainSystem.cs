using GameFramework.Core;
using GameFramework.SituationSystems;
using System.Collections;
using UnityEngine;

namespace dtank
{
	/// <summary>
	/// dtankのメインシステム
	/// </summary>
	public class DTankMainSystem : MainSystem
	{
		private SceneSituationContainer _sceneSituationContainer = new SceneSituationContainer();


		protected override IEnumerator RebootRoutineInternal(object[] args)
		{
			Debug.Log("Begin RebootRoutineInternal()");

			Debug.Log("End RebootRoutineInternal()");

			yield break;
		}

		protected override IEnumerator StartRoutineInternal(object[] args)
		{
			Debug.Log("Begin StartRoutineInternal()");

			var t = _sceneSituationContainer.Transition(new TitleSceneSituation());
			if (t.Exception != null)
			{
				Debug.LogError(t.Exception.ToString());
				yield break;
			}

			Debug.Log(t.ToString());

			Debug.Log("End StartRoutineInternal()");

			yield break;
		}

		protected override void UpdateInternal()
		{
			_sceneSituationContainer.Update();
		}

		protected override void LateUpdateInternal()
		{
			_sceneSituationContainer.LateUpdate();
		}

		protected override void OnDestroyInternal()
		{
			Debug.Log("OnDestroyInternal()");

			_sceneSituationContainer.Dispose();
			_sceneSituationContainer = null;
		}
	}
}
