using GameFramework.Core;

namespace dtank
{
    public class BattleStatePlaying : BattleStateBase
    {
        public override BattleState Key => BattleState.Playing;

        private BattlePlayingPresenter _presenter;

        public override void OnEnter(BattleState prevKey, IScope scope)
        {
            var model = BattleModel.Get();
            var uiView = Services.Get<BattleUiView>();

            var playingUiView = uiView.PlayingUiView;
            var fadeController = Services.Get<FadeController>();
            playingUiView.Setup(fadeController);

            _presenter = new BattlePlayingPresenter(model,
                playingUiView,
                uiView.PlayerStatusUiView,
                uiView.TankControlUiView);
            _presenter.ScopeTo(scope);

            _presenter.Activate();
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnExit(BattleState nextKey)
        {
            _presenter.Deactivate();
        }
    }
}