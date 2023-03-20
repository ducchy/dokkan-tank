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
        private readonly DisposableScope _scope;

        public TitlePresenter(TitleUiView uiView, TitleCamera camera, TitleModel model)
        {
            Debug.Log("TitlePresenter.TitlePresenter()");

            _uiView = uiView;
            _camera = camera;
            _model = model;
            _scope = new DisposableScope();

            SetEvent();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        public void OnChangeState(TitleState prev, TitleState current)
        {
            switch (current)
            {
                case TitleState.Idle:
                    _camera.Play();
                    break;
                case TitleState.Start:
                    _model.EndScene();
                    break;
            }
        }

        private void SetEvent()
        {
            _uiView.OnStartButtonClickAsObservable
                .TakeUntil(_scope)
                .Subscribe(_ => _model.PushStart());
        }
    }
}