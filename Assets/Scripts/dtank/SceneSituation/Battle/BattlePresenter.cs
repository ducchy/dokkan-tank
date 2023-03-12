namespace dtank
{
    public class BattlePresenter
    {
        private readonly BattleController _controller;
        private readonly BattleTankPresenter _tankPresenter;

        public BattlePresenter(
            BattleController controller, 
            BattleTankPresenter tankPresenter)
        {
            _controller = controller;
            _tankPresenter = tankPresenter;
        }

        public void Update()
        {
            _controller.Update();
        }
    }
}