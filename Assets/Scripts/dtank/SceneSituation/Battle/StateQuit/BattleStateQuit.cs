using GameFramework.Core;

namespace dtank.StateQuit
{
    public class BattleStateQuit : BattleStateBase
    {
        public override BattleState Key => BattleState.Quit;

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