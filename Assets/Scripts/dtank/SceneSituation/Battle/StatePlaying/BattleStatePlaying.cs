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
            Debug.Log("BattleStatePlaying.OnEnter()");

            var model = BattleModel.Get();
            var uiView = Services.Get<BattleUiView>();

            _presenter = new BattlePlayingPresenter(model, uiView, uiView.PlayingUiView,
                uiView.TankStatusUiView,
                uiView.TankControlUiView);
            _presenter.ScopeTo(scope);
            
            _presenter.Activate();
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnExit(BattleState nextKey)
        {
            Debug.Log("BattleStatePlaying.OnExit()");

            _presenter.Deactivate();
        }
    }
}