using System;
using GameFramework.Core;
using UniRx;

namespace dtank
{
    public class BattlePlayingPresenter : IDisposable
    {
        private readonly BattleModel _model;
        private readonly BattleUiView _uiView;
        private readonly BattlePlayingUiView _playingUiView;
        private readonly BattleTankStatusUiView _statusUiView;
        private readonly BattleTankControlUiView _controlUiView;
        private readonly DisposableScope _scope = new();

        public BattlePlayingPresenter(
            BattleModel model,
            BattleUiView uiView,
            BattlePlayingUiView playingUiView, 
            BattleTankStatusUiView statusUiView,
            BattleTankControlUiView controlUiView)
        {
            _model = model;
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
            _playingUiView.Start();
            _statusUiView.Open();
            _controlUiView.Open();
        }

        public void Deactivate()
        {
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
            _playingUiView.OnEndFinishAsObservable
                .TakeUntil(_scope)
                .Subscribe(_ => _uiView.EndPlaying())
                .ScopeTo(_scope);
            
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
    }
}