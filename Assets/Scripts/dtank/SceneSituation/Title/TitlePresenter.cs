using System;
using GameFramework.Core;
using UniRx;

namespace dtank
{
    public class TitlePresenter : IDisposable
    {
        private readonly TitleUiView _uiView;
        private readonly TitleCameraController _cameraController;
        private readonly TitleModel _model;
        private readonly DisposableScope _scope = new();

        public TitlePresenter(TitleUiView uiView, TitleCameraController cameraController, TitleModel model)
        {
            _uiView = uiView;
            _cameraController = cameraController;
            _model = model;
            
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
                        case TitleState.Idle:
                            _cameraController.Play();
                            break;
                        case TitleState.Start:
                            _model.ChangeState(TitleState.End);
                            break;
                    }
                })
                .ScopeTo(_scope);
        }

        private void SetEvent()
        {
            _uiView.OnStartButtonClickAsObservable
                .TakeUntil(_scope)
                .Subscribe(_ => _model.ChangeState(TitleState.Start))
                .ScopeTo(_scope);
        }
    }
}