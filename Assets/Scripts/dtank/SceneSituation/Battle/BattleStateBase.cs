using GameFramework.Core;
using GameFramework.StateSystems;

namespace dtank
{
    public abstract class BattleStateBase : IState<BattleState>
    {
        public abstract BattleState Key { get; }

        public abstract void OnEnter(BattleState prevKey, IScope scope);
        public abstract void OnExit(BattleState nextKey);
        public abstract void OnUpdate(float deltaTime);
    }
}
