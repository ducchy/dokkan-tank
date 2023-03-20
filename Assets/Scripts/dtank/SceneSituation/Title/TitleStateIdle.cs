using GameFramework.Core;
using UnityEngine;

namespace dtank
{
    public class TitleStateIdle : TitleStateBase
    {
        public override TitleState Key => TitleState.Idle;

        public override void OnEnter(TitleState prevKey, IScope scope)
        {
            Debug.Log("TitleStateIdle.OnEnter()");
        }

        public override void OnUpdate(float deltaTime)
        {
            // Debug.Log("TitleStateIdle.OnUpdate()");
        }

        public override void OnExit(TitleState nextKey)
        {
            Debug.Log("TitleStateIdle.OnExit()");
        }
    }
}