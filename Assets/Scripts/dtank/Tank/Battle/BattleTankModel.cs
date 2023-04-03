using GameFramework.ModelSystems;
using GameFramework.TaskSystems;
using UniRx;
using UnityEngine;

namespace dtank
{
    public class BattleTankModel : AutoIdModel<BattleTankModel>
    {
        #region Variable
        
        public BattleTankActorModel ActorModel { get; private set; }

        private readonly ReactiveProperty<BattleTankState> _currentState = new(BattleTankState.Invalid);

        public IReadOnlyReactiveProperty<BattleTankState> CurrentState => _currentState;

        private readonly ReactiveProperty<int> _hp = new();
        public IReadOnlyReactiveProperty<int> Hp => _hp;

        private readonly ReactiveProperty<float> _moveAmount = new();
        public IReactiveProperty<float> MoveAmount => _moveAmount;

        private readonly ReactiveProperty<float> _turnAmount = new();
        public IReactiveProperty<float> TurnAmount => _turnAmount;

        private readonly ReactiveProperty<bool> _invincibleFlag = new(false);
        public IReactiveProperty<bool> InvincibleFlag => _invincibleFlag;

        private readonly ReactiveProperty<int> _score = new();
        public IReadOnlyReactiveProperty<int> Score => _score;

        private readonly ReactiveProperty<int> _rank = new();
        public IReadOnlyReactiveProperty<int> Rank => _rank;

        public string Name { get; private set; }
        public int BodyId { get; private set; }
        public CharacterType CharacterType { get; private set; }
        public BattleTankParameterData ParameterData { get; private set; }
        public string AssetKey => $"{BodyId:D3}";

        public Vector3 Position { get; private set; }
        public Vector3 Forward { get; private set; }
        public bool DeadFlag => _hp.Value <= 0;
        public bool IsMovable => _currentState.Value == BattleTankState.FreeMove;

        private float _inputMoveAmount;
        private float _inputTurnAmount;
        private float _invincibleRemainTime;
        private ITask _taskImplementation;
        private bool _isActive;
        
        #endregion Variable

        private BattleTankModel(int id) : base(id)
        {
            ActorModel = BattleTankActorModel.Create();
        }

        protected override void OnDeletedInternal()
        {
            BattleTankActorModel.Delete(ActorModel.Id);
            ActorModel = null;
        }

        public void Setup(string name, int bodyId, CharacterType characterType, BattleTankParameterData parameterData)
        {
            Name = name;
            BodyId = bodyId;
            CharacterType = characterType;
            ParameterData = parameterData;
        }

        public void ResetParameter()
        {
            _hp.Value = ParameterData.hp;
        }

        public void Update()
        {
            UpdateOnInvincible(Time.deltaTime);
        }

        public void Damage(IAttacker attacker)
        {
            if (_invincibleFlag.Value)
                return;

            if (_currentState.Value == BattleTankState.Damage)
                return;

            if (_hp.Value <= 0)
                return;

            _hp.Value--;
            attacker?.DealDamage();

            if (_hp.Value == 0)
            {
                SetState(BattleTankState.Dead);
                return;
            }

            BeginInvincible();
            SetState(BattleTankState.Damage);
        }
        
        public void IncrementScore()
        {
            _score.Value++;
        }
        
        #region Setter

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
            SetMoveAmount();
        }

        public void SetInputTurnAmount(float inputTurnAmount)
        {
            _inputTurnAmount = inputTurnAmount;
            SetMoveAmount();
        }
        private void SetMoveAmount()
        {
            _moveAmount.Value = IsMovable ? _inputMoveAmount : 0f;
            _turnAmount.Value = IsMovable ? _inputTurnAmount : 0f;
        }
        
        #endregion Setter

        #region State

        public void SetState(BattleTankState state)
        {
            if (!CheckTransitionState(state))
                return;
            
            _currentState.Value = state;
            SetMoveAmount();
        }

        private bool CheckTransitionState(BattleTankState next)
        {
            var current = _currentState.Value;
            if (next == current)
                return false;

            bool IsLastState(BattleTankState state) => state is BattleTankState.Dead or BattleTankState.Result;

            if (IsLastState(current))
                return false;

            if (IsLastState(next))
                return true;

            switch (current)
            {
                case BattleTankState.Damage:
                case BattleTankState.Ready:
                    if (next != BattleTankState.FreeMove)
                        return false;
                    break;
                case BattleTankState.ShotCurve:
                case BattleTankState.ShotStraight:
                    if (next != BattleTankState.FreeMove && next != BattleTankState.Damage)
                        return false;
                    break;
            }

            return true;
        }
        
        #endregion State
        
        #region Invincible

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

        private void UpdateOnInvincible(float deltaTime)
        {
            if (!_invincibleFlag.Value)
                return;

            _invincibleRemainTime -= deltaTime;
            if (_invincibleRemainTime <= 0f)
                EndInvincible();
        }
        
        #endregion Invincible
    }
}