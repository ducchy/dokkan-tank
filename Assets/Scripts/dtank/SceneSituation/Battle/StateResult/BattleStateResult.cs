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
            var model = BattleModel.Get();
            var uiView = Services.Get<BattleUiView>();
            var controller = Services.Get<BattleCameraController>();
            
            var resultUiView = Services.Get<BattleResultUiView>();
            resultUiView.Construct();

            var resultController = new BattleResultController(model.RuleModel, resultUiView);
            
            _presenter = new BattleResultPresenter(model, controller, resultController, uiView, resultUiView);
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