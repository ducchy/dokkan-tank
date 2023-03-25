using System;
using GameFramework.ModelSystems;
using GameFramework.TaskSystems;
using UniRx;
using UnityEngine;

namespace dtank
{
    public class BattleTankModel : AutoIdModel<BattleTankModel>
    {   
        public BattleTankActorModel ActorModel { get; private set; }
        
        public event Action<BattleTankModel> OnUpdated;

        private readonly ReactiveProperty<BattleTankState> _battleState =
            new ReactiveProperty<BattleTankState>(BattleTankState.Invalid);

        public IReadOnlyReactiveProperty<BattleTankState> BattleState => _battleState;

        private readonly ReactiveProperty<int> _hp = new ReactiveProperty<int>();
        public IReadOnlyReactiveProperty<int> Hp => _hp;

        private readonly ReactiveProperty<float> _moveAmount = new ReactiveProperty<float>();
        public IReactiveProperty<float> MoveAmount => _moveAmount;

        private readonly ReactiveProperty<float> _turnAmount = new ReactiveProperty<float>();
        public IReactiveProperty<float> TurnAmount => _turnAmount;

        private readonly ReactiveProperty<bool> _invincibleFlag = new ReactiveProperty<bool>(false);
        public IReactiveProperty<bool> InvincibleFlag => _invincibleFlag;

        public string Name { get; private set; }
        public int ModelId { get; private set; }
        public CharacterType CharacterType { get; private set; }
        public TransformData StartPointData { get; private set; }
        public BattleTankParameterData ParameterData { get; private set; }

        public Vector3 Position { get; private set; }
        public Vector3 Forward { get; private set; }
        public bool DeadFlag => _hp.Value <= 0;

        private float _inputMoveAmount;
        private float _inputTurnAmount;
        private float _invincibleRemainTime;
        private ITask _taskImplementation;
        private bool _isActive;

        private BattleTankModel(int id) : base(id)
        {
            ActorModel = BattleTankActorModel.Create();
        }

        protected override void OnDeletedInternal()
        {
            BattleTankActorModel.Delete(ActorModel.Id);
            ActorModel = null;
        }

        public void Update(string name, int modelId, CharacterType characterType, TransformData startPointData,
            BattleTankParameterData parameterData)
        {
            Name = name;
            ModelId = modelId;
            CharacterType = characterType;
            StartPointData = startPointData;
            ParameterData = parameterData;

            OnUpdated?.Invoke(this);
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
            _hp.SetValueAndForceNotify(ParameterData.hp);
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

            _invincibleRemainTime = ParameterData.invincibleDuration;
            _invincibleFlag.Value = true;
        }

        private void EndInvincible()
        {
            _invincibleFlag.Value = false;
        }
    }
}