using GameFramework.Core;
using UnityEngine;

namespace dtank
{
    public class BattleStatePlaying : BattleStateBase
    {
        public override BattleState Key => BattleState.Playing;

        private readonly BattlePlayingPresenter _presenter;

        public BattleStatePlaying()
        {
            var uiView = Services.Get<BattlePlayingUiView>();
            uiView.Construct();

            var controller = new BattlePlayingController();
            
            _presenter = new BattlePlayingPresenter(controller, uiView);
            _presenter.OnEnd = () => StateContainer?.Change(BattleState.Result);
        }
        
        public override void OnEnter(BattleState prevKey, IScope scope)
        {
            Debug.Log("BattleStatePlaying.OnEnter()");

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