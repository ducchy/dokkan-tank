using System;
using GameFramework.Core;
using UniRx;

namespace dtank
{
    public class BattlePresenter : IDisposable
    {
        private readonly BattleModel _model;
        private readonly BattleUiView _uiView;
        private readonly BattleCameraController _cameraController;
        private readonly DisposableScope _scope = new();

        public BattlePresenter(
            BattleModel model,
            BattleUiView uiView, 
            BattleCameraController cameraController)
        {
            _model = model;
            _uiView = uiView;
            _cameraController = cameraController;

            Bind();
            SetEvent();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        private void Bind()
        {
            _model.CurrentState
                .TakeUntil(_scope)
                .Subscribe(state =>
                {
                    switch (state)
                    {
                        case BattleState.Ready:
                            _cameraController.PlayReady();
                            _uiView.Reset();
                            break;
                    }
                })
                .ScopeTo(_scope);
        }

        private void SetEvent()
        {
            _cameraController.OnEndReadyAsObservable
                .TakeUntil(_scope)
                .Subscribe(_ => _model.ChangeState(BattleState.Playing))
                .ScopeTo(_scope);
        }
    }
}