using GameFramework.Core;
using UnityEngine;

namespace dtank
{
    public class BattleStateReady : BattleStateBase
    {
        public override BattleState Key => BattleState.Ready;

        private BattleReadyPresenter _presenter;

        public override void OnEnter(BattleState prevKey, IScope scope)
        {
            Debug.Log("BattleStateReady.OnEnter()");

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
            Debug.Log("BattleStateReady.OnExit()");

            _presenter.Deactivate();
            _presenter = null;
        }
    }
}