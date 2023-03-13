using UniRx;
using UnityEngine;

namespace dtank
{
    public class BattleTankModel : TankModelBase
    {
        private readonly ReactiveProperty<BattleTankState> _battleState =
            new ReactiveProperty<BattleTankState>(BattleTankState.Invalid);

        public IReadOnlyReactiveProperty<BattleTankState> BattleState => _battleState;

        private readonly ReactiveProperty<int> _hp = new ReactiveProperty<int>(3);
        public IReadOnlyReactiveProperty<int> Hp => _hp;

        public readonly TransformData StartPointData;

        public Vector3 Position { get; private set; }
        public Vector3 Forward { get; private set; }
        public bool DeadFlag => _hp.Value <= 0;

        public BattleTankModel(TransformData startPointData)
        {
            StartPointData = startPointData;
        }

        public void SetPosition(Vector3 position)
        {
            Position = position;
        }

        public void SetForward(Vector3 forward)
        {
            Forward = forward;
        }

        public void Ready()
        {
            _hp.Value = 3;
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