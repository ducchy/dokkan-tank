using System;
using GameFramework.Core;
using UniRx;

namespace dtank
{
    public class BattlePresenter : IDisposable
    {
        private readonly BattleModel _model;
        private readonly BattleController _controller;
        private readonly PlayerBattleTankPresenter _playerTankPresenter;
        private readonly NpcBattleTankPresenter[] _npcTankPresenters;
        private readonly DokkanTankRulePresenter _rulePresenter;

        private readonly DisposableScope _scope;

        public BattlePresenter(
            BattleModel model,
            BattleController controller,
            PlayerBattleTankPresenter playerTankPresenter,
            NpcBattleTankPresenter[] npcTankPresenters,
            DokkanTankRulePresenter rulePresenter)
        {
            _model = model;
            _controller = controller;
            _playerTankPresenter = playerTankPresenter;
            _npcTankPresenters = npcTankPresenters;
            _rulePresenter = rulePresenter;

            _scope = new DisposableScope();

            Bind();
        }

        public void Update(float deltaTime)
        {
            _controller.Update(deltaTime);
            _playerTankPresenter.Update(deltaTime);
            foreach (var npcTankPresenter in _npcTankPresenters)
                npcTankPresenter.Update(deltaTime);
            _rulePresenter.Update(deltaTime);
        }

        public void Dispose()
        {
            _playerTankPresenter.Dispose();
            foreach (var npcTankPresenter in _npcTankPresenters)
                npcTankPresenter.Dispose();
            _rulePresenter.Dispose();
            
            _scope.Dispose();
        }

        private void Bind()
        {
            _model.State.TakeUntil(_scope).Subscribe(state =>
            {
                _playerTankPresenter.OnChangedState(state);
                foreach (var npcTankPresenter in _npcTankPresenters)
                    npcTankPresenter.OnChangedState(state);
                _rulePresenter.OnChangedState(state);
            });
    }

        public void OnChangedState(BattleState prev, BattleState current)
        {
        }
    }
}