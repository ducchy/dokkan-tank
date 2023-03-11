using System;
using UniRx;

namespace dtank
{
    public class BattlePlayingPresenter : IDisposable
    {
        private readonly BattlePlayingUiView _uiView;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public Action OnEnd = null;
        
        public BattlePlayingPresenter(BattlePlayingUiView uiView)
        {
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
            _uiView.SetActive(true);
        }

        public void Deactivate()
        {            
            _uiView.SetActive(false);
        }
    }
}