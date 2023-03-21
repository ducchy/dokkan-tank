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
        private readonly PlayerBattleTankPresenter _playerTankPresenter;
        private readonly NpcBattleTankPresenter[] _npcTankPresenters;
        private readonly TankActorContainer _tankActorContainer;
        private readonly DisposableScope _scope = new DisposableScope();

        public BattlePresenter(
            BattleModel model,
            BattleUiView uiView,
            BattleCameraController cameraController,
            PlayerBattleTankPresenter playerTankPresenter,
            NpcBattleTankPresenter[] npcTankPresenters,
            TankActorContainer tankActorContainer)
        {
            _model = model;
            _uiView = uiView;
            _cameraController = cameraController;
            _playerTankPresenter = playerTankPresenter;
            _npcTankPresenters = npcTankPresenters;
            _tankActorContainer = tankActorContainer;

            Bind();
            SetEvent();
        }

        public void Dispose()
        {
            _playerTankPresenter.Dispose();
            foreach (var npcTankPresenter in _npcTankPresenters)
                npcTankPresenter.Dispose();

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
        }

        private void SetEvent()
        {
            _cameraController.OnEndReadyAsObservable
                .TakeUntil(_scope)
                .Subscribe(_ => _model.ChangeState(BattleState.Playing))
                .ScopeTo(_scope);

            foreach (var tankActor in _tankActorContainer.ActorDictionary.Values)
                tankActor.OnDealDamageAsObservable
                    .TakeUntil(_scope)
                    .Subscribe(_ => _model.RuleModel.IncrementScore(tankActor.OwnerId))
                    .ScopeTo(_scope);

            _uiView.PlayingUiView.OnForceEndAsObservable
                .TakeUntil(_scope)
                .Subscribe(_ => _model.RuleModel.ForceEnd())
                .ScopeTo(_scope);
        }
    }
}