using UniRx;

namespace dtank
{
    public class BattleTankModel : TankModelBase
    {
        private readonly ReactiveProperty<BattleTankState> _battleState =
            new ReactiveProperty<BattleTankState>(BattleTankState.Invalid);
        public IReadOnlyReactiveProperty<BattleTankState> BattleState => _battleState;

        private readonly ReactiveProperty<int> _hp = new ReactiveProperty<int>(3);
        public IReadOnlyReactiveProperty<int> Hp => _hp;

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
            if (_battleState.Value == BattleTankState.Damage)
                return;
            
            if (_hp.Value <= 0)
                return;

            _battleState.Value = BattleTankState.Damage;
            _hp.Value--;
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