using System;
using GameFramework.Core;
using GameFramework.SituationSystems;
using GameFramework.StateSystems;
using UniRx;

namespace dtank
{
    public class BattlePresenter : IDisposable
    {
        private readonly BattleUiView _uiView;

        private readonly StateContainer<BattleStateBase, BattleState> _stateContainer;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        
        public BattlePresenter(BattleUiView uiView)
        {
            _uiView = uiView;
            
            _stateContainer = Services.Get<StateContainer<BattleStateBase, BattleState>>();
            _stateContainer.OnChangedState += OnChangeState;

            var situationContainer = Services.Get<SceneSituationContainer>();

            uiView.OnClickQuitButtonAsObservable
                .Subscribe(_ => situationContainer.Transition(new TitleSceneSituation()))
                .AddTo(_disposable);
            
            uiView.OnClickRetryButtonAsObservable
                .Subscribe(_ => situationContainer.Transition(new BattleSceneSituation()))
                .AddTo(_disposable);
        }
        
        public void Dispose()
        {
            _stateContainer.OnChangedState -= OnChangeState;
            _disposable.Dispose();
        }

        private void OnChangeState(BattleState prev, BattleState current)
        {
            switch (current)
            {
            }
        }
    }
}