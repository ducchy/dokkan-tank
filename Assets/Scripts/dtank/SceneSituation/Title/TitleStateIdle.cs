using GameFramework.Core;
using UniRx;

namespace dtank
{
    public class TitleStateIdle : TitleStateBase
    {
        public override TitleState Key => TitleState.Idle;

        private readonly DisposableScope _scope = new();

        public override void OnEnter(TitleState prevKey, IScope scope)
        {
            var model = TitleModel.Get();
            
            var uiView = Services.Get<TitleUiView>();
            uiView.OnStartButtonClickObservable
                .TakeUntil(_scope)
                .Subscribe(_ => model.ChangeState(TitleState.Exit))
                .ScopeTo(_scope);
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