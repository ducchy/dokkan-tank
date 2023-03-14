namespace dtank
{
    public class BattleController
    {
        private readonly BattleCamera _camera;
        private readonly BattleTankModel _playerTankModel;
        private readonly TankActorContainer _tankActorContainer;
        
        public BattleController(
            BattleCamera camera,
            BattleTankModel playerTankModel,
            TankActorContainer tankActorContainer)
        {
            _camera = camera;
            _playerTankModel = playerTankModel;
            _tankActorContainer = tankActorContainer;
        }

        public void PlayReady()
        {
            var player = _tankActorContainer.ActorDictionary[_playerTankModel.PlayerId];
            _camera.SetFollowTarget(player.transform);
            _camera.PlayReady();
        }

        public void PlayResult(int winnerId)
        {
            var winner = _tankActorContainer.ActorDictionary[winnerId];
            _camera.SetFollowTarget(winner.transform);
            _camera.PlayResult();
        }

        public void EndResult()
        {
            _camera.EndResult();
        }

        public void Update(float deltaTime)
        {   
            _camera.OnUpdate(deltaTime);
        }
    }
}