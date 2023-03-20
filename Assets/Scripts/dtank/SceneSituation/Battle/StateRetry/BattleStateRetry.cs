using GameFramework.Core;

namespace dtank.StateRetry
{
    public class BattleStateRetry : BattleStateBase
    {
        public override BattleState Key => BattleState.Retry;
        
        public override void OnEnter(BattleState prevKey, IScope scope)
        {
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnExit(BattleState nextKey)
        {
        }
    }
}