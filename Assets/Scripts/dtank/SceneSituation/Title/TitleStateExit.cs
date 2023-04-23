using GameFramework.Core;
using UniRx;

namespace dtank
{
    public class TitleStateExit : TitleStateBase
    {
        public override TitleState Key => TitleState.Exit;

        private readonly DisposableScope _scope = new();

        public override void OnEnter(TitleState prevKey, IScope scope)
        {
            var model = TitleModel.Get();
            
            var uiView = Services.Get<TitleUiView>();
            uiView.CompleteExitObservable
                .TakeUntil(_scope)
                .Subscribe(_ => model.ChangeState(TitleState.ToBattle))
                .ScopeTo(_scope);
            
            uiView.PlayExit();
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