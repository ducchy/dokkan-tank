using System;

namespace dtank
{
    public class PlayerBattleTankPresenter : BattleTankPresenterBase
    {
        private readonly BattleTankControlUiView _controlUiView;
        private readonly BattleTankStatusUiView _statusUiView;

        public Action OnGameOver;

        public PlayerBattleTankPresenter(
            BattleTankController controller,
            BattleTankModel model,
            BattleTankActor actor,
            BattleTankControlUiView controlUiView,
            BattleTankStatusUiView statusUiView)
            : base(controller, model, actor, controlUiView)
        {
            _controlUiView = controlUiView;
            _statusUiView = statusUiView;
            
            Bind();
            SetEvents();
        }

        public override void Dispose()
        {
            base.Dispose();
            
            OnGameOver = null;
        }

        protected override void OnHpChanged(int hp)
        {
            base.OnHpChanged(hp);

            _statusUiView.SetHp(hp);
        }

        protected override void OnDead()
        {
            base.OnDead();
            
            OnGameOver?.Invoke();
        }

        public override void OnChangedState(BattleState prev, BattleState current)
        {
            base.OnChangedState(prev, current);
            
            _controlUiView.SetActive(current == BattleState.Playing);
        }
    }
}