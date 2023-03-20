using GameFramework.Core;
using UnityEngine;

namespace dtank
{
    public class BattleStateResult : BattleStateBase
    {
        public override BattleState Key => BattleState.Result;

        private readonly BattleResultPresenter _presenter;

        public BattleStateResult()
        {
            var ruleModel = BattleRuleModel.Get();
            var uiView = Services.Get<BattleUiView>();
            var controller = Services.Get<BattleController>();
            
            var resultUiView = Services.Get<BattleResultUiView>();
            resultUiView.Construct();

            var resultController = new BattleResultController(ruleModel, resultUiView);
            
            _presenter = new BattleResultPresenter(ruleModel, controller, resultController, uiView, resultUiView);
            _presenter.OnQuit = () => StateContainer.Change(BattleState.Quit);
            _presenter.OnRetry = () => StateContainer.Change(BattleState.Retry);
        }
        
        public override void OnEnter(BattleState prevKey, IScope scope)
        {
            Debug.Log("BattleStateResult.OnEnter()");

            _presenter.Activate();
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnExit(BattleState nextKey)
        {
            Debug.Log("BattleStateResult.OnExit()");
            
            _presenter.Deactivate();
        }
    }
}