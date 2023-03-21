using GameFramework.TaskSystems;
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

        private readonly ReactiveProperty<float> _moveAmount = new ReactiveProperty<float>(0f);
        public IReactiveProperty<float> MoveAmount => _moveAmount;

        private readonly ReactiveProperty<float> _turnAmount = new ReactiveProperty<float>(0f);
        public IReactiveProperty<float> TurnAmount => _turnAmount;

        private readonly ReactiveProperty<bool> _invincibleFlag = new ReactiveProperty<bool>(false);
        public IReactiveProperty<bool> InvincibleFlag => _invincibleFlag;

        public readonly TankData Data;
        public readonly TransformData StartPointData;
        
        public Vector3 Position { get; private set; }
        public Vector3 Forward { get; private set; }
        public bool DeadFlag => _hp.Value <= 0;

        private float _inputMoveAmount;
        private float _inputTurnAmount;
        private float _invincibleRemainTime;
        private ITask _taskImplementation;
        private bool _isActive;

        public BattleTankModel(TankData data, TransformData startPointData)
        {
            Data = data;
            StartPointData = startPointData;
        }

        public override void Dispose()
        {
        }

        public void Update()
        {
            UpdateOnInvincible(Time.deltaTime);
        }

        public void SetPosition(Vector3 position)
        {
            Position = position;
        }

        public void SetForward(Vector3 forward)
        {
            Forward = forward;
        }

        public void SetInputMoveAmount(float inputMoveAmount)
        {
            _inputMoveAmount = inputMoveAmount;

            if (!IsMovableState(_battleState.Value))
                return;

            _moveAmount.Value = inputMoveAmount;
        }

        public void SetInputTurnAmount(float inputTurnAmount)
        {
            _inputTurnAmount = inputTurnAmount;

            if (!IsMovableState(_battleState.Value))
                return;

            _turnAmount.Value = inputTurnAmount;
        }

        public void Ready()
        {
            _hp.Value = 3;
            SetState(BattleTankState.Ready);
        }

        public void Playing()
        {
            SetState(BattleTankState.FreeMove);
        }

        public void ShotCurve()
        {
            if (_battleState.Value != BattleTankState.FreeMove)
                return;

            SetState(BattleTankState.ShotCurve);
        }

        public void EndShotCurve()
        {
            if (_battleState.Value != BattleTankState.ShotCurve)
                return;

            SetState(BattleTankState.FreeMove);
        }

        public void ShotStraight()
        {
            if (_battleState.Value != BattleTankState.FreeMove)
                return;

            SetState(BattleTankState.ShotStraight);
        }

        public void EndShotStraight()
        {
            if (_battleState.Value != BattleTankState.ShotStraight)
                return;

            SetState(BattleTankState.FreeMove);
        }

        public void Damage(IAttacker attacker)
        {
            if (_invincibleFlag.Value)
                return;

            if (_battleState.Value == BattleTankState.Damage)
                return;

            if (_hp.Value <= 0)
                return;

            BeginInvincible();
            SetState(BattleTankState.Damage);
            _hp.Value--;
            attacker?.DealDamage();
        }

        public void EndDamage()
        {
            if (_battleState.Value != BattleTankState.Damage)
                return;

            if (DeadFlag)
            {
                Dead();
                return;
            }

            SetState(BattleTankState.FreeMove);
        }

        private void Dead()
        {
            EndInvincible();
            SetState(BattleTankState.Dead);
        }

        public void Result()
        {
            SetState(BattleTankState.Result);
        }

        private bool IsMovableState(BattleTankState state)
        {
            switch (state)
            {
                case BattleTankState.FreeMove:
                    return true;
                default:
                    return false;
            }
        }

        private void SetMovable(bool flag)
        {
            _moveAmount.Value = flag ? _inputMoveAmount : 0f;
            _turnAmount.Value = flag ? _inputTurnAmount : 0f;
        }

        private void SetState(BattleTankState state)
        {
            _battleState.Value = state;
            SetMovable(IsMovableState(state));
        }

        private void UpdateOnInvincible(float deltaTime)
        {
            if (!_invincibleFlag.Value)
                return;

            _invincibleRemainTime -= deltaTime;
            if (_invincibleRemainTime <= 0f)
                EndInvincible();
        }

        private void BeginInvincible()
        {
            if (_invincibleFlag.Value)
                return;

            _invincibleRemainTime = Data.InvincibleDuration;
            _invincibleFlag.Value = true;
        }

        private void EndInvincible()
        {
            _invincibleFlag.Value = false;
        }
    }
}