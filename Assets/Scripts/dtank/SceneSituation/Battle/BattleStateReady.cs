using GameFramework.Core;
using UnityEngine;

namespace dtank
{
    public class BattleStateReady : BattleStateBase
    {
        public override BattleState Key => BattleState.Ready;

        public override void OnEnter(BattleState prevKey, IScope scope)
        {
            Debug.Log("BattleStateReady.OnEnter()");
        }

        public override void OnUpdate(float deltaTime)
        {
            // Debug.Log("BattleStateReady.OnUpdate()");
        }

        public override void OnExit(BattleState nextKey)
        {
            Debug.Log("BattleStateReady.OnExit()");
        }
    }
}