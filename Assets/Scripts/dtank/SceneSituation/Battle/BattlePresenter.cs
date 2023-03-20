using System;
using GameFramework.Core;
using UniRx;

namespace dtank
{
    public class BattlePresenter : IDisposable
    {
        private readonly BattleModel _model;
        private readonly BattleCameraController _cameraController;
        private readonly PlayerBattleTankPresenter _playerTankPresenter;
        private readonly NpcBattleTankPresenter[] _npcTankPresenters;
        private readonly BattleTankActor[] _tankActors;
        private readonly DisposableScope _scope = new DisposableScope();

        public BattlePresenter(
            BattleModel model,
            BattleCameraController cameraController,
            PlayerBattleTankPresenter playerTankPresenter,
            NpcBattleTankPresenter[] npcTankPresenters, 
            BattleTankActor[] tankActors)
        {
            _model = model;
            _cameraController = cameraController;
            _playerTankPresenter = playerTankPresenter;
            _npcTankPresenters = npcTankPresenters;
            _tankActors = tankActors;

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
                            break;
                    }

                    _playerTankPresenter.OnChangedState(state);
                    foreach (var npcTankPresenter in _npcTankPresenters)
                        npcTankPresenter.OnChangedState(state);
                })
                .ScopeTo(_scope);
        }

        private void SetEvent()
        {
            _cameraController.OnEndReadyAction = () => { _model.ChangeState(BattleState.Playing); };
            
            foreach (var tankActor in _tankActors)
                tankActor.OnDealDamageListener = () => _model.RuleModel.IncrementScore(tankActor.OwnerId);
        }
    }
}