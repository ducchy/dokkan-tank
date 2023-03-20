using System;
using GameFramework.Core;
using UniRx;

namespace dtank
{
    public class BattleReadyPresenter : IDisposable
    {
        private readonly BattleCameraController _cameraController;
        private readonly BattleUiView _uiView;
        private readonly BattleReadyUiView _readyUiView;
        private readonly DisposableScope _scope = new DisposableScope();
        
        public BattleReadyPresenter(
            BattleCameraController cameraController, 
            BattleUiView uiView,
            BattleReadyUiView readyUiView)
        {
            _cameraController = cameraController;
            _uiView = uiView;
            _readyUiView = readyUiView;

            SetEvent();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        public void Activate()
        { 
            _uiView.BeginBattle();
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
            
            _uiView.OnBeginBattleAction = OnBeginBattle;
        }

        private void OnBeginBattle()
        {
            _readyUiView.BeginReady();
        }
    }
}