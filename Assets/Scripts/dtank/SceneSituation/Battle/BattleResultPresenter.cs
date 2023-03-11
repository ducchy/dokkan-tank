using System;
using UniRx;

namespace dtank
{
    public class BattleResultPresenter : IDisposable
    {
        private readonly BattleResultUiView _uiView;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public Action OnRetry = null;
        public Action OnQuit = null;
        
        public BattleResultPresenter(BattleResultUiView uiView)
        {
            _uiView = uiView;

            _uiView.OnQuitObservable
                .Subscribe(_ => OnQuit?.Invoke())
                .AddTo(_disposable);

            _uiView.OnRetryObservable
                .Subscribe(_ => OnRetry?.Invoke())
                .AddTo(_disposable);
        }

        public void Dispose()
        {
            _disposable.Dispose();
            
            OnRetry = null;
            OnQuit = null;
        }

        public void Activate(bool winFlag)
        {
            _uiView.PlayResult(winFlag);
        }

        public void Deactivate()
        {
            _uiView.SetActive(false);
        }
    }
}