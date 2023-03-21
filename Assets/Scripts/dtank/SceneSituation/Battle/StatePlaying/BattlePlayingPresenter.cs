using System;
using GameFramework.Core;
using UniRx;

namespace dtank
{
    public class BattlePlayingPresenter : IDisposable
    {
        private readonly BattleModel _model;
        private readonly BattlePlayingController _controller;
        private readonly BattleUiView _uiView;
        private readonly BattlePlayingUiView _playingUiView;
        private readonly BattleTankStatusUiView _statusUiView;
        private readonly BattleTankControlUiView _controlUiView;
        private readonly DisposableScope _scope = new DisposableScope();

        public BattlePlayingPresenter(
            BattleModel model,
            BattlePlayingController controller,
            BattleUiView uiView,
            BattlePlayingUiView playingUiView, 
            BattleTankStatusUiView statusUiView,
            BattleTankControlUiView controlUiView)
        {
            _model = model;
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
            _scope.Dispose();
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
            _model.RuleModel.ResultType
                .TakeUntil(_scope)
                .Subscribe(OnResultTypeChanged)
                .ScopeTo(_scope);

            _model.RuleModel.RemainTime
                .TakeUntil(_scope)
                .Subscribe(_playingUiView.SetTime)
                .ScopeTo(_scope);
        }

        private void SetEvent()
        {
            _playingUiView.OnEndFinishListener = OnEndFinish;
            
            _uiView.OnEndPlayingAsObservable
                .TakeUntil(_scope)
                .Subscribe(_ => _model.ChangeState(BattleState.Result))
                .ScopeTo(_scope);
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
    }
}