using GameFramework.Core;
using UniRx;

namespace dtank
{
    public class TitleStateEnter : TitleStateBase
    {
        public override TitleState Key => TitleState.Enter;

        private readonly DisposableScope _scope = new();

        public override void OnEnter(TitleState prevKey, IScope scope)
        {
            var model = TitleModel.Get();
            
            var uiView = Services.Get<TitleUiView>();
            uiView.CompleteEnterObservable
                .TakeUntil(_scope)
                .Subscribe(_ => model.ChangeState(TitleState.Idle))
                .ScopeTo(_scope);
            
            uiView.PlayEnter();
            
            var camera = Services.Get<TitleCameraController>();
            camera.Play();
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnExit(TitleState nextKey)
        {
            _scope.Dispose();
        }
    }
}