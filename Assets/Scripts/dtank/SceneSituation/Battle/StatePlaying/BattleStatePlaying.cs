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
            var model = BattleModel.Get();
            var uiView = Services.Get<BattleUiView>();

            var playingUiView = Services.Get<BattlePlayingUiView>();
            playingUiView.Setup();

            var controller = new BattlePlayingController();

            var statusUiView = Services.Get<BattleTankStatusUiView>();
            var controlUiView = Services.Get<BattleTankControlUiView>();

            _presenter = new BattlePlayingPresenter(model, controller, uiView, playingUiView, statusUiView,
                controlUiView);
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