using System.Collections;
using UnityEngine;

namespace dtank
{
	/// <summary>
	/// dtankのメインシステム
	/// </summary>
	public class DTankMainSystem : GameFramework.Core.MainSystem
	{
		protected override IEnumerator RebootRoutineInternal(object[] args)
		{
			Debug.Log("Begin RebootRoutineInternal()");

			Debug.Log("End RebootRoutineInternal()");

			yield break;
		}

		protected override IEnumerator StartRoutineInternal(object[] args)
		{
			Debug.Log("Begin StartRoutineInternal()");

			Debug.Log("End StartRoutineInternal()");

			yield break;
		}

		protected override void UpdateInternal()
		{
		}

		protected override void LateUpdateInternal()
		{
		}

		protected override void OnDestroyInternal()
		{
			Debug.Log("OnDestroyInternal()");
		}
	}
}
