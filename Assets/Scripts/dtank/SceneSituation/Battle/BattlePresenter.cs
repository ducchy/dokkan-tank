using System;
using GameFramework.Core;
using UniRx;

namespace dtank
{
    public class BattlePresenter : IDisposable
    {
        private readonly BattleModel _model;
        private readonly BattleUiView _uiView;
        private readonly BattleCameraController _cameraController;
        private readonly BattleTankEntityContainer _tankEntityContainer;
        private readonly DisposableScope _scope = new DisposableScope();

        public BattlePresenter(
            BattleModel model,
            BattleUiView uiView, 
            BattleTankEntityContainer tankEntityContainer, 
            BattleCameraController cameraController)
        {
            _model = model;
            _uiView = uiView;
            _tankEntityContainer = tankEntityContainer;
            _cameraController = cameraController;

            Bind();
            SetEvent();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        private void Bind()
        {
            _model.CurrentState
                .TakeUntil(_scope)
                .Subscribe(state =>
                {
                    switch (state)
                    {
                        case BattleState.Ready:
                            _cameraController.PlayReady();
                            _uiView.Reset();
                            break;
                    }
                })
                .ScopeTo(_scope);
            
            _model.MainPlayerTankModel.Hp
                .TakeUntil(_scope)
                .Subscribe(_uiView.TankStatusUiView.SetHp)
                .ScopeTo(_scope);
        }

        private void SetEvent()
        {
            _cameraController.OnEndReadyAsObservable
                .TakeUntil(_scope)
                .Subscribe(_ => _model.ChangeState(BattleState.Playing))
                .ScopeTo(_scope);

            /*
            foreach (var tankActor in _tankActorContainer.ActorDictionary.Values)
                tankActor.OnDealDamageAsObservable
                    .TakeUntil(_scope)
                    .Subscribe(_ => _model.RuleModel.IncrementScore(tankActor.OwnerId))
                    .ScopeTo(_scope);
            */

            _uiView.PlayingUiView.OnForceEndAsObservable
                .TakeUntil(_scope)
                .Subscribe(_ => _model.RuleModel.ForceEnd())
                .ScopeTo(_scope);
        }
    }
}