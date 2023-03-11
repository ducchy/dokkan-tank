using System;
using UniRx;

namespace dtank
{
    public class BattleReadyPresenter : IDisposable
    {
        private readonly BattleReadyUiView _uiView;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public Action OnStartPlaying = null;
        
        public BattleReadyPresenter(BattleReadyUiView uiView)
        {
            _uiView = uiView;

            _uiView.OnStartObservable
                .Subscribe(_ => OnStartPlaying?.Invoke())
                .AddTo(_disposable);
        }

        public void Dispose()
        {
            _disposable.Dispose();
            
            OnStartPlaying = null;
        }

        public void Activate()
        {
            _uiView.PlayCountDown();
        }

        public void Deactivate()
        {
        }
    }
}