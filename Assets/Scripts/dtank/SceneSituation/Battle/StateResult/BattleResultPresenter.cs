using System;
using UniRx;

namespace dtank
{
    public class BattleResultPresenter : IDisposable
    {
        private readonly BattleRuleModel _ruleModel;
        private readonly BattleController _controller;
        private readonly BattleResultController _resultController;
        private readonly BattleUiView _uiView;
        private readonly BattleResultUiView _resultUiView;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public Action OnRetry = null;
        public Action OnQuit = null;

        private Action _onEnd;
        
        public BattleResultPresenter(
            BattleRuleModel ruleModel,
            BattleController controller,
            BattleResultController resultController,
            BattleUiView uiView,
            BattleResultUiView resultUiView)
        {
            _ruleModel = ruleModel;
            _controller = controller;
            _resultController = resultController;
            _uiView = uiView;
            _resultUiView = resultUiView;

            SetEvent();
        }

        public void Dispose()
        {
            _disposable.Dispose();
            
            OnRetry = null;
            OnQuit = null;
            _onEnd = null;
        }

        private void SetEvent()
        {
            _resultUiView.OnQuitButtonClickedListener = OnQuitButtonClicked;
            _resultUiView.OnRetryButtonClickedListener = OnRetryButtonClicked;

            _uiView.OnBeginResultListener = OnBeginResult;
            _uiView.OnEndBattleListener = OnEndBattle;
        }

        public void Activate()
        {
            _controller.PlayResult(_ruleModel.WinnerId);
            _uiView.BeginResult();
        }

        public void Deactivate()
        {
            _controller.EndResult();
            _resultUiView.SetActive(false);
        }

        private void OnQuitButtonClicked()
        {
            _onEnd = OnQuit;
            _uiView.EndBattle();
        }

        private void OnRetryButtonClicked()
        {
            _onEnd = OnRetry;
            _uiView.EndBattle();
        }

        private void OnBeginResult()
        {
            _resultController.PlayResult();
        }

        private void OnEndBattle()
        {
            _onEnd?.Invoke();
        }
    }
}