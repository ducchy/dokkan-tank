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
        private readonly BattleUiView _uiView;
        private readonly BattleResultUiView _resultUiView;
        private readonly DisposableScope _scope = new DisposableScope();

        public BattleResultPresenter(
            BattleModel model,
            BattleCameraController cameraController,
            BattleResultController resultController,
            BattleUiView uiView,
            BattleResultUiView resultUiView)
        {
            _model = model;
            _cameraController = cameraController;
            _resultController = resultController;
            _uiView = uiView;
            _resultUiView = resultUiView;
            
            SetEvent();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        private void SetEvent()
        {
            _resultUiView.OnQuitButtonClickedListener = OnQuitButtonClicked;
            _resultUiView.OnRetryButtonClickedListener = OnRetryButtonClicked;

            _uiView.OnBeginResultAsObservable
                .TakeUntil(_scope)
                .Subscribe(_ => _resultController.PlayResult())
                .ScopeTo(_scope);
        }

        public void Activate()
        {
            _cameraController.PlayResult(_model.RuleModel.WinnerId);
            _uiView.BeginResult();
        }

        public void Deactivate()
        {
            _cameraController.EndResult();
            _resultUiView.SetActive(false);
        }

        private void OnQuitButtonClicked()
        {
            _model.ChangeState(BattleState.Quit);
        }

        private void OnRetryButtonClicked()
        {
            _model.ChangeState(BattleState.Retry);
        }
    }
}