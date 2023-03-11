using GameFramework.Core;
using UnityEngine;

namespace dtank
{
    public class BattleStateReady : BattleStateBase
    {
        public override BattleState Key => BattleState.Ready;

        private readonly BattleReadyPresenter _presenter;

        public BattleStateReady()
        {
            var uiView = Services.Get<BattleReadyUiView>();
            uiView.Construct();
            
            _presenter = new BattleReadyPresenter(uiView);
            _presenter.OnStartPlaying = () => StateContainer?.Change(BattleState.Playing);
        }
        
        public override void OnEnter(BattleState prevKey, IScope scope)
        {
            Debug.Log("BattleStateReady.OnEnter()");

            _presenter.Activate();
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnExit(BattleState nextKey)
        {
            Debug.Log("BattleStateReady.OnExit()");
            
            _presenter.Deactivate();
        }
    }
}