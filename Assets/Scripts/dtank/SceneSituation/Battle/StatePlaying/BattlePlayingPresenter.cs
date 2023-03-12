using System;
using UniRx;

namespace dtank
{
    public class BattlePlayingPresenter : IDisposable
    {
        private readonly BattlePlayingController _controller;
        private readonly BattlePlayingUiView _uiView;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public Action OnEnd = null;
        
        public BattlePlayingPresenter(
            BattlePlayingController controller,
            BattlePlayingUiView uiView, 
            PlayerBattleTankPresenter playerTankPresenter)
        {
            _controller = controller;
            _uiView = uiView;

            _uiView.OnEndObservable
                .Subscribe(_ => OnEnd?.Invoke())
                .AddTo(_disposable);
        }

        public void Dispose()
        {
            _disposable.Dispose();
            
            OnEnd = null;
        }

        public void Activate()
        {
            _controller.Activate();
            
            _uiView.SetActive(true);
        }

        public void Deactivate()
        {            
            _controller.Deactivate();
            
            _uiView.SetActive(false);
        }
    }
}