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
            var uiView = Services.Get<BattleUiView>();

            var playingUiView = Services.Get<BattlePlayingUiView>();
            playingUiView.Construct();

            var controller = new BattlePlayingController();

            var ruleModel = BattleRuleModel.Get();
            var statusUiView = Services.Get<BattleTankStatusUiView>();
            var controlUiView = Services.Get<BattleTankControlUiView>();

            _presenter = new BattlePlayingPresenter(ruleModel, controller, uiView, playingUiView, statusUiView,
                controlUiView);
            _presenter.OnFinished = () => StateContainer?.Change(BattleState.Result);
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