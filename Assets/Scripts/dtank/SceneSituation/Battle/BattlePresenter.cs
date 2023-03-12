namespace dtank
{
    public class BattlePresenter
    {
        private readonly BattleController _controller;
        private readonly PlayerBattleTankPresenter _playerTankPresenter;
        private readonly NpcBattleTankPresenter[] _npcTankPresenters;

        public BattlePresenter(
            BattleController controller, 
            PlayerBattleTankPresenter playerTankPresenter,
            NpcBattleTankPresenter[] npcTankPresenters)
        {
            _controller = controller;
            _playerTankPresenter = playerTankPresenter;
            _npcTankPresenters = npcTankPresenters;
        }

        public void Update(float deltaTime)
        {
            _controller.Update(deltaTime);
            _playerTankPresenter.Update(deltaTime);
        }
    }
}