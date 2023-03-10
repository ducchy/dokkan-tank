using GameFramework.Core;
using UnityEngine;

namespace dtank
{
    public class BattleStatePlaying : BattleStateBase
    {
        public override BattleState Key => BattleState.Playing;

        public override void OnEnter(BattleState prevKey, IScope scope)
        {
            Debug.Log("BattleStatePlaying.OnEnter()");
        }

        public override void OnUpdate(float deltaTime)
        {
            // Debug.Log("BattleStatePlaying.OnUpdate()");
        }

        public override void OnExit(BattleState nextKey)
        {
            Debug.Log("BattleStatePlaying.OnExit()");
        }
    }
}