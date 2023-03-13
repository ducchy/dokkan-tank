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
            foreach (var npcTankPresenter in _npcTankPresenters)
                npcTankPresenter.Update(deltaTime);
        }

        public void Dispose()
        {
            _playerTankPresenter.Dispose();
            foreach (var npcTankPresenter in _npcTankPresenters)
                npcTankPresenter.Dispose();
            _rulePresenter.Dispose();
        }
    }
}