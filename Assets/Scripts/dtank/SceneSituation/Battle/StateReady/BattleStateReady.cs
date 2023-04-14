using GameFramework.Core;
using UniRx;

namespace dtank
{
    public class BattleStateReady : BattleStateBase
    {
        public override BattleState Key => BattleState.Ready;

        private BattleCameraController _cameraController;
        private BattleReadyUiView _readyUiView;
        
        public override void OnEnter(BattleState prevKey, IScope scope)
        {
            _cameraController = Services.Get<BattleCameraController>();
            
            var uiView = Services.Get<BattleUiView>();
            _readyUiView = uiView.ReadyUiView;

            SetEvent(scope);
            
            _readyUiView.Begin();
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnExit(BattleState nextKey)
        {
            _readyUiView.End();
        }

        private void SetEvent(IScope scope)
        {
            _readyUiView.OnSkipButtonClickObservable
                .TakeUntil(scope)
                .Subscribe(_ => _cameraController.SkipReady())
                .ScopeTo(scope);
        }
    }
}