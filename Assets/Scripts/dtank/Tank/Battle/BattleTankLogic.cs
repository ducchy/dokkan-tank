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
        private readonly IBattlePlayerStatusUiView _playerStatusUiView;
        private readonly TankBehaviourController _controller;

        private readonly DisposableScope _scope = new();

        public BattleTankLogic(
            BattleModel model,
            BattleTankModel tankModel,
            BattleTankActor tankActor,
            IBattlePlayerStatusUiView playerStatusUiView, 
            TankBehaviourController controller)
        {
            _model = model;
            _tankModel = tankModel;
            _tankActor = tankActor;
            _playerStatusUiView = playerStatusUiView;
            _controller = controller;

            _controller.ScopeTo(_scope);
        }

        protected override void UpdateInternal()
        {
            _controller.Update();
        }

        protected override void DisposeInternal()
        {
            _scope.Dispose();
        }

        protected override void ActivateInternal(IScope scope)
        {
            Bind();
            SetEvents();
        }

        private void Bind()
        {
            _model.CurrentState
                .TakeUntil(_scope)
                .Subscribe(OnChangedBattleState)
                .ScopeTo(_scope);
            
            _tankModel.CurrentState
                .TakeUntil(_scope)
                .Subscribe(OnChangedBattleTankState)
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

            _tankActor.OnDealDamageAsObservable
                .TakeUntil(_scope)
                .Subscribe(_ => _tankModel.IncrementScore())
                .ScopeTo(_scope);
        }

        private void OnChangedBattleTankState(BattleTankState state)
        {
            switch (state)
            {
                case BattleTankState.Damage:
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
                    break;
            }
        }

        private void OnAnimatorStateExit(BattleTankAnimatorState animState)
        {
            switch (animState)
            {
                case BattleTankAnimatorState.Damage:
                    _tankModel.SetState(BattleTankState.FreeMove);
                    break;
                case BattleTankAnimatorState.ShotCurve:
                    _tankModel.SetState(BattleTankState.FreeMove);
                    break;
                case BattleTankAnimatorState.ShotStraight:
                    _tankModel.SetState(BattleTankState.FreeMove);
                    break;
            }
        }

        private void OnChangedBattleState(BattleState state)
        {
            switch (state)
            {
                case BattleState.Ready:
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