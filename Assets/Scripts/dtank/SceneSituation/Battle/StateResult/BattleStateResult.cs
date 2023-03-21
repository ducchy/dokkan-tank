using GameFramework.Core;
using UnityEngine;

namespace dtank
{
    public class BattleStateResult : BattleStateBase
    {
        public override BattleState Key => BattleState.Result;

        private BattleResultPresenter _presenter;

        public override void OnEnter(BattleState prevKey, IScope scope)
        {
            Debug.Log("BattleStateResult.OnEnter()");
            
            var model = BattleModel.Get();
            var uiView = Services.Get<BattleUiView>();
            var resultController = new BattleResultController(model.RuleModel, uiView.ResultUiView);
            resultController.ScopeTo(scope);
            
            var controller = Services.Get<BattleCameraController>();
            _presenter = new BattleResultPresenter(model, controller, resultController, uiView, uiView.ResultUiView);
            _presenter.ScopeTo(scope);

            _presenter.Activate();
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnExit(BattleState nextKey)
        {
            Debug.Log("BattleStateResult.OnExit()");
            
            _presenter.Deactivate();
            _presenter = null;
        }
    }
}