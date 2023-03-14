namespace dtank
{
    public class PlayerBattleTankPresenter : BattleTankPresenterBase
    {
        private readonly BattleTankStatusUiView _statusUiView;

        public PlayerBattleTankPresenter(
            BattleTankController controller,
            BattleTankModel model,
            BattleTankActor actor,
            BattleTankControlUiView controlUiView,
            BattleTankStatusUiView statusUiView)
            : base(controller, model, actor, controlUiView)
        {
            _statusUiView = statusUiView;
            
            Bind();
            SetEvents();
        }
        
        protected override void OnHpChanged(int hp)
        {
            base.OnHpChanged(hp);

            _statusUiView.SetHp(hp);
        }
    }
}