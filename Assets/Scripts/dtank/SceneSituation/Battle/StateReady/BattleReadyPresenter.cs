using System;
using UniRx;

namespace dtank
{
    public class BattleReadyPresenter : IDisposable
    {
        private readonly BattleCamera _camera;
        private readonly BattleController _controller;
        private readonly BattleUiView _uiView;
        private readonly BattleReadyUiView _readyUiView;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        
        public Action OnStartPlaying;

        public BattleReadyPresenter(
            BattleCamera camera,
            BattleController controller, 
            BattleUiView uiView,
            BattleReadyUiView readyUiView)
        {
            _camera = camera;
            _controller = controller;
            _uiView = uiView;
            _readyUiView = readyUiView;

            SetEvent();
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        public void Activate()
        { 
            _controller.PlayReady();
            _uiView.BeginBattle();
        }

        public void Deactivate()
        {
            _readyUiView.EndReady();
        }

        private void SetEvent()
        {
            _camera.OnEndReady = OnEndReady;
            _readyUiView.OnSkipButtonClickedListener = _camera.SkipReady;
            _uiView.OnBeginBattleListener = OnBeginBattle;
        }

        private void OnEndReady()
        {
            OnStartPlaying.Invoke();
        }

        private void OnBeginBattle()
        {
            _readyUiView.BeginReady();
        }
    }
}