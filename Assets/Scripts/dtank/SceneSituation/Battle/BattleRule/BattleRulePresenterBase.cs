using System;

namespace dtank
{
    public abstract class BattleRulePresenterBase : IDisposable
    {
        public abstract void Dispose();

        public abstract void OnChangedState(BattleState prev, BattleState current);
    }
}