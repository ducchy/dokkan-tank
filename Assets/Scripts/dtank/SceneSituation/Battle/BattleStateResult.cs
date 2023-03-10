using GameFramework.Core;
using UnityEngine;

namespace dtank
{
    public class BattleStateResult : BattleStateBase
    {
        public override BattleState Key => BattleState.Result;

        public override void OnEnter(BattleState prevKey, IScope scope)
        {
            Debug.Log("BattleStateResult.OnEnter()");
        }

        public override void OnUpdate(float deltaTime)
        {
            // Debug.Log("BattleStateResult.OnUpdate()");
        }

        public override void OnExit(BattleState nextKey)
        {
            Debug.Log("BattleStateResult.OnExit()");
        }
    }
}