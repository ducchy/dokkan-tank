using System;
using GameFramework.Core;
using UniRx;
using UnityEngine;

namespace dtank
{
    public class TitlePresenter : IDisposable
    {
        private readonly TitleUiView _uiView;
        private readonly TitleCamera _camera;
        private readonly TitleModel _model;
        private readonly DisposableScope _scope = new DisposableScope();

        public TitlePresenter(TitleUiView uiView, TitleCamera camera, TitleModel model)
        {
            Debug.Log("TitlePresenter.TitlePresenter()");

            _uiView = uiView;
            _camera = camera;
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
                            _camera.Play();
                            break;
                        case TitleState.Start:
                            _model.EndScene();
                            break;
                    }
                })
                .ScopeTo(_scope);
        }

        private void SetEvent()
        {
            _uiView.OnStartButtonClickAsObservable
                .TakeUntil(_scope)
                .Subscribe(_ => _model.PushStart())
                .ScopeTo(_scope);
        }
    }
}