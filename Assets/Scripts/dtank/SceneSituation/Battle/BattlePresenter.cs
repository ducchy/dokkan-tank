using System;

namespace dtank
{
    public class BattlePresenter : IDisposable
    {
        private readonly BattleController _controller;
        private readonly PlayerBattleTankPresenter _playerTankPresenter;
        private readonly NpcBattleTankPresenter[] _npcTankPresenters;
        private readonly DokkanTankRulePresenter _rulePresenter;

        public BattlePresenter(
            BattleController controller,
            PlayerBattleTankPresenter playerTankPresenter,
            NpcBattleTankPresenter[] npcTankPresenters,
            DokkanTankRulePresenter rulePresenter)
        {
            _controller = controller;
            _playerTankPresenter = playerTankPresenter;
            _npcTankPresenters = npcTankPresenters;
            _rulePresenter = rulePresenter;
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
        }

        public void OnChangedState(BattleState prev, BattleState current)
        {
            _playerTankPresenter.OnChangedState(prev, current);
            foreach (var npcTankPresenter in _npcTankPresenters)
                npcTankPresenter.OnChangedState(prev, current);
            _rulePresenter.OnChangedState(prev, current);
        }
    }
}