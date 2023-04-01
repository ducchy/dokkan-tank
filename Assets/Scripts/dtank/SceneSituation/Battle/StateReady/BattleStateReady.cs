using GameFramework.Core;

namespace dtank
{
    public class BattleStateReady : BattleStateBase
    {
        public override BattleState Key => BattleState.Ready;

        private BattleReadyPresenter _presenter;

        public override void OnEnter(BattleState prevKey, IScope scope)
        {
            var controller = Services.Get<BattleCameraController>();
            var uiView = Services.Get<BattleUiView>();
            _presenter = new BattleReadyPresenter(controller, uiView.ReadyUiView);
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