using GameFramework.Core;
using UnityEngine;

namespace dtank
{
    public class TitleStateStart : TitleStateBase
    {
        public override TitleState Key => TitleState.Start;

        public override void OnEnter(TitleState prevKey, IScope scope)
        {
            Debug.Log("TitleStateStart.OnEnter()");
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnExit(TitleState nextKey)
        {
            Debug.Log("TitleStateStart.OnExit()");
        }
    }
}