using UniRx;

namespace dtank
{
    public class BattleTankModel : TankModelBase
    {
        private readonly ReactiveProperty<BattleTankState> _battleState =
            new ReactiveProperty<BattleTankState>(BattleTankState.Invalid);

        public IReadOnlyReactiveProperty<BattleTankState> BattleState => _battleState;

        public BattleTankModel()
        {
        }

        public void Ready()
        {
            _battleState.Value = BattleTankState.Ready;
        }

        public void Playing()
        {
            _battleState.Value = BattleTankState.FreeMove;
        }

        public void ShotCurve()
        {
            if (_battleState.Value != BattleTankState.FreeMove)
                return;

            _battleState.Value = BattleTankState.ShotCurve;
        }

        public void EndShotCurve()
        {
            if (_battleState.Value != BattleTankState.ShotCurve)
                return;

            _battleState.Value = BattleTankState.FreeMove;
        }

        public void ShotStraight()
        {
            if (_battleState.Value != BattleTankState.FreeMove)
                return;

            _battleState.Value = BattleTankState.ShotStraight;
        }

        public void EndShotStraight()
        {
            if (_battleState.Value != BattleTankState.ShotStraight)
                return;

            _battleState.Value = BattleTankState.FreeMove;
        }

        public void Damage()
        {
            _battleState.Value = BattleTankState.Damage;
        }

        public void EndDamage()
        {
            if (_battleState.Value != BattleTankState.Damage)
                return;

            _battleState.Value = BattleTankState.FreeMove;
        }

        public void Result()
        {
            _battleState.Value = BattleTankState.Result;
        }
    }
}