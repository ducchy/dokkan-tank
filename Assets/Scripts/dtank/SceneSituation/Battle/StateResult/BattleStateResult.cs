using GameFramework.Core;

namespace dtank
{
    public class BattleStateResult : BattleStateBase
    {
        public override BattleState Key => BattleState.Result;

        private BattleResultPresenter _presenter;

        public override void OnEnter(BattleState prevKey, IScope scope)
        {
            var model = BattleModel.Get();
            var cameraController = Services.Get<BattleCameraController>();
            
            var uiView = Services.Get<BattleUiView>();
            var resultUiView = uiView.ResultUiView;
            var fadeController = Services.Get<FadeController>();
            resultUiView.Setup(fadeController);
            
            var resultController = new BattleResultController(model.RuleModel, resultUiView);
            resultController.ScopeTo(scope);
            
            _presenter = new BattleResultPresenter(model, cameraController, resultController, resultUiView);
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