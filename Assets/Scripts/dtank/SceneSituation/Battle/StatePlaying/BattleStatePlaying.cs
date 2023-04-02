using GameFramework.Core;
using UnityEngine;

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

            _presenter = new BattlePlayingPresenter(model, uiView, uiView.PlayingUiView,
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