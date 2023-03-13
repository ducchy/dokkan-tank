namespace dtank
{
    public class BattleController
    {
        private readonly FollowTargetCamera _camera;
        
        public BattleController(FollowTargetCamera camera)
        {
            _camera = camera;
        }

        public void Update(float deltaTime)
        {   
            _camera.OnUpdate(deltaTime);
        }
    }
}