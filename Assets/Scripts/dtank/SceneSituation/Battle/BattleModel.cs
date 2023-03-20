using GameFramework.ModelSystems;
using UniRx;

namespace dtank
{
    public class BattleModel : SingleModel<BattleModel>
    {
        private ReactiveProperty<BattleState> _state = new ReactiveProperty<BattleState>();
        public IReadOnlyReactiveProperty<BattleState> State => _state;

        private BattleModel(object empty) : base(empty)
        {
        }

        public void OnChangedState(BattleState prev, BattleState current)
        {
            _state.Value = current;
        }
    }
}