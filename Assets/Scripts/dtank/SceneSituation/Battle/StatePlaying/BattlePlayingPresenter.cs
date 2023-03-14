using System;
using UniRx;

namespace dtank
{
    public class BattlePlayingPresenter : IDisposable
    {
        private readonly BattleRuleModel _ruleModel;
        private readonly BattlePlayingController _controller;
        private readonly BattleUiView _uiView;
        private readonly BattlePlayingUiView _playingUiView;
        private readonly BattleTankStatusUiView _statusUiView;
        private readonly BattleTankControlUiView _controlUiView;
        
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public Action OnFinished;
        
        public BattlePlayingPresenter(
            BattleRuleModel ruleModel,
            BattlePlayingController controller,
            BattleUiView uiView,
            BattlePlayingUiView playingUiView, 
            BattleTankStatusUiView statusUiView,
            BattleTankControlUiView controlUiView)
        {
            _ruleModel = ruleModel;
            _controller = controller;
            _uiView = uiView;
            _playingUiView = playingUiView;
            _statusUiView = statusUiView;
            _controlUiView = controlUiView;

            Bind();
            SetEvent();
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        public void Activate()
        {
            _controller.Activate();
            
            _playingUiView.Start();
            _statusUiView.Open();
            _controlUiView.Open();
        }

        public void Deactivate()
        {
            _controller.Deactivate();
        }

        private void Bind()
        {
            _ruleModel.ResultType.Subscribe(OnResultTypeChanged).AddTo(_disposable);
        }

        private void SetEvent()
        {
            _playingUiView.OnEndFinishListener = OnEndFinish;
            _uiView.OnEndPlayingListener = OnEndPlaying;
        }

        private void OnResultTypeChanged(BattleResultType type)
        {
            if (type == BattleResultType.None)
                return;
            
            _playingUiView.Finish();
            _statusUiView.Close();
            _controlUiView.Close();
        }

        private void OnEndFinish()
        {
            _uiView.EndPlaying();
        }

        private void OnEndPlaying()
        {
            OnFinished?.Invoke();
        }
    }
}