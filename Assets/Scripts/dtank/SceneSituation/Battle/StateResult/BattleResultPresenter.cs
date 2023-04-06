using System;
using GameFramework.Core;
using UniRx;

namespace dtank
{
    public class BattleResultPresenter : IDisposable
    {
        private readonly BattleModel _model;
        private readonly BattleCameraController _cameraController;
        private readonly BattleResultController _resultController;
        private readonly BattleResultUiView _resultUiView;
        private readonly DisposableScope _scope = new();

        public BattleResultPresenter(
            BattleModel model,
            BattleCameraController cameraController,
            BattleResultController resultController,
            BattleResultUiView resultUiView)
        {
            _model = model;
            _cameraController = cameraController;
            _resultController = resultController;
            _resultUiView = resultUiView;
            
            SetEvent();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        private void SetEvent()
        {
            _resultUiView.OnQuitButtonClickedAsObservable
                .TakeUntil(_scope)
                .Subscribe(_ => _model.ChangeState(BattleState.Quit))
                .ScopeTo(_scope);
            
            _resultUiView.OnRetryButtonClickedAsObservable
                .TakeUntil(_scope)
                .Subscribe(_ =>  _model.ChangeState(BattleState.Retry))
                .ScopeTo(_scope);
        }

        public void Activate()
        {
            _cameraController.PlayResult(_model.RuleModel.TopPlayerId);
            _resultController.PlayResult();
        }

        public void Deactivate()
        {
            _cameraController.EndResult();
            _resultUiView.SetActive(false);
        }
    }
}