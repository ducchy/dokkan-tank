using System;
using GameFramework.Core;
using UniRx;

namespace dtank
{
    public class BattleReadyPresenter : IDisposable
    {
        private readonly BattleCameraController _cameraController;
        private readonly BattleReadyUiView _readyUiView;
        private readonly DisposableScope _scope = new();
        
        public BattleReadyPresenter(
            BattleCameraController cameraController, 
            BattleReadyUiView readyUiView)
        {
            _cameraController = cameraController;
            _readyUiView = readyUiView;

            SetEvent();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        public void Activate()
        { 
            _readyUiView.BeginReady();
        }

        public void Deactivate()
        {
            _readyUiView.EndReady();
        }

        private void SetEvent()
        {
            _readyUiView.OnSkipButtonClickAsObservable
                .TakeUntil(_scope)
                .Subscribe(_ => _cameraController.SkipReady())
                .ScopeTo(_scope);
        }
    }
}