using GameFramework.Core;
using GameFramework.EntitySystems;
using UniRx;

namespace dtank
{
    public class BattleTankLogic : EntityLogic
    {
        private readonly BattleModel _model;
        private readonly BattleTankModel _tankModel;
        private readonly BattleTankActor _tankActor;
        private readonly BattleTankControlUiView _controlUiView;
        private readonly IBehaviourSelector _npcBehaviourSelector;
        private readonly IBattlePlayerStatusUiView _playerStatusUiView;

        private IBehaviourSelector _currentBehaviourSelector;

        private readonly DisposableScope _scope = new();
        private readonly DisposableScope _behaviourScope = new();

        public BattleTankLogic(
            BattleModel model,
            BattleTankModel tankModel,
            BattleTankActor tankActor,
            BattleTankControlUiView controlUiView,
            IBehaviourSelector npcBehaviourSelector,
            IBattlePlayerStatusUiView playerStatusUiView)
        {
            _model = model;
            _tankModel = tankModel;
            _tankActor = tankActor;
            _controlUiView = controlUiView;
            _npcBehaviourSelector = npcBehaviourSelector;
            _playerStatusUiView = playerStatusUiView;

            SetBehaviourSelector(tankModel.CharacterType);
        }

        protected override void UpdateInternal()
        {
            _currentBehaviourSelector?.Update();
        }

        protected override void DisposeInternal()
        {
            _scope.Dispose();
            _behaviourScope.Dispose();
        }

        protected override void ActivateInternal(IScope scope)
        {
            Bind();
            SetEvents();
        }

        private void Bind()
        {
            _tankModel.CurrentState
                .TakeUntil(_scope)
                .Subscribe(OnStateChanged)
                .ScopeTo(_scope);

            _tankModel.MoveAmount
                .TakeUntil(_scope)
                .Subscribe(_tankActor.SetMoveAmount)
                .ScopeTo(_scope);

            _tankModel.TurnAmount
                .TakeUntil(_scope)
                .Subscribe(_tankActor.SetTurnAmount)
                .ScopeTo(_scope);

            _tankModel.InvincibleFlag
                .TakeUntil(_scope)
                .Subscribe(_tankActor.SetInvincible)
                .ScopeTo(_scope);

            _tankModel.Hp
                .TakeUntil(_scope)
                .Subscribe(_playerStatusUiView.SetHp)
                .ScopeTo(_scope);

            _tankModel.Score
                .TakeUntil(_scope)
                .Subscribe(_playerStatusUiView.SetScore)
                .ScopeTo(_scope);

            _tankModel.Rank
                .TakeUntil(_scope)
                .Subscribe(_playerStatusUiView.SetRank)
                .ScopeTo(_scope);
            
            _tankModel.DeadFlag
                .TakeUntil(_scope)
                .Subscribe(_playerStatusUiView.SetDeadFlag)
                .ScopeTo(_scope);

            if (_controlUiView != null)
            {
                _controlUiView.OnAutoToggleValueChangedAsObservable
                    .TakeUntil(_scope)
                    .Subscribe(autoFlag =>
                        SetBehaviourSelector(autoFlag ? CharacterType.NonPlayer : CharacterType.Player))
                    .ScopeTo(_scope);
            }
        }

        private void SetEvents()
        {
            _tankActor.OnStateExitAsObservable
                .TakeUntil(_scope)
                .Subscribe(OnAnimatorStateExit)
                .ScopeTo(_scope);

            _tankActor.OnDamageReceivedAsObservable
                .TakeUntil(_scope)
                .Subscribe(_tankModel.Damage)
                .ScopeTo(_scope);

            _tankActor.OnPositionChangedAsObservable
                .TakeUntil(_scope)
                .Subscribe(_tankModel.SetPosition)
                .ScopeTo(_scope);

            _tankActor.OnForwardChangedAsObservable
                .TakeUntil(_scope)
                .Subscribe(_tankModel.SetForward)
                .ScopeTo(_scope);

            _tankActor.OnDealDamageAsObservable
                .TakeUntil(_scope)
                .Subscribe(_ => _tankModel.IncrementScore())
                .ScopeTo(_scope);
        }

        private void SetBehaviourSelector(CharacterType characterType)
        {
            if (characterType == CharacterType.Player && _controlUiView != null)
            {
                SetBehaviourSelector(_controlUiView);
                return;
            }

            SetBehaviourSelector(_npcBehaviourSelector);
        }

        private void SetBehaviourSelector(IBehaviourSelector behaviourSelector)
        {
            _currentBehaviourSelector = behaviourSelector;

            _behaviourScope.Dispose();

            _model.CurrentState
                .TakeUntil(_behaviourScope)
                .Subscribe(OnChangedState)
                .ScopeTo(_behaviourScope);

            _model.RuleModel.ResultType
                .TakeUntil(_scope)
                .Subscribe(result =>
                {
                    if (result != BattleResultType.None)
                        _currentBehaviourSelector.SetActive(false);
                })
                .ScopeTo(_scope);

            behaviourSelector.OnDamageAsObservable
                .TakeUntil(_behaviourScope)
                .Subscribe(_tankModel.Damage)
                .ScopeTo(_behaviourScope);

            behaviourSelector.OnShotCurveAsObservable
                .TakeUntil(_behaviourScope)
                .Subscribe(_ => _tankModel.SetState(BattleTankState.ShotCurve))
                .ScopeTo(_behaviourScope);

            behaviourSelector.OnShotStraightAsObservable
                .TakeUntil(_behaviourScope)
                .Subscribe(_ => _tankModel.SetState(BattleTankState.ShotStraight))
                .ScopeTo(_behaviourScope);

            behaviourSelector.OnTurnValueChangedAsObservable
                .TakeUntil(_behaviourScope)
                .Subscribe(_tankModel.SetInputTurnAmount)
                .ScopeTo(_behaviourScope);

            behaviourSelector.OnMoveValueChangedAsObservable
                .TakeUntil(_behaviourScope)
                .Subscribe(_tankModel.SetInputMoveAmount)
                .ScopeTo(_behaviourScope);
        }

        private void OnStateChanged(BattleTankState state)
        {
            switch (state)
            {
                case BattleTankState.Damage:
                    _currentBehaviourSelector.BeginDamage();
                    _tankActor.Damage();
                    break;
                case BattleTankState.ShotCurve:
                    _tankActor.ShotCurve();
                    break;
                case BattleTankState.ShotStraight:
                    _tankActor.ShotStraight();
                    break;
                case BattleTankState.Dead:
                    _tankActor.Dead();
                    _currentBehaviourSelector.SetActive(false);
                    break;
            }
        }

        private void OnAnimatorStateExit(BattleTankAnimatorState animState)
        {
            switch (animState)
            {
                case BattleTankAnimatorState.Damage:
                    _tankModel.SetState(BattleTankState.FreeMove);
                    _currentBehaviourSelector.EndDamage();
                    break;
                case BattleTankAnimatorState.ShotCurve:
                    _tankModel.SetState(BattleTankState.FreeMove);
                    break;
                case BattleTankAnimatorState.ShotStraight:
                    _tankModel.SetState(BattleTankState.FreeMove);
                    _currentBehaviourSelector.EndShotStraight();
                    break;
            }
        }

        private void OnAnimationEvent(string id)
        {
            switch (id)
            {
                case "ShotCurve":
                    _tankActor.ShotCurve();
                    break;
                case "ShotStraight":
                    _tankActor.ShotStraight();
                    break;
            }
        }

        private void OnChangedState(BattleState state)
        {
            _currentBehaviourSelector.SetActive(state == BattleState.Playing);

            switch (state)
            {
                case BattleState.Ready:
                    _currentBehaviourSelector.Reset();
                    _tankModel.SetState(BattleTankState.Ready);
                    break;
                case BattleState.Playing:
                    _tankModel.SetState(BattleTankState.FreeMove);
                    break;
                case BattleState.Result:
                    _tankModel.SetState(BattleTankState.Result);
                    break;
            }
        }
    }
}