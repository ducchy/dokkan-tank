namespace dtank
{
    public class BattleController
    {
        private readonly FollowTargetCamera _camera;
        
        public BattleController(FollowTargetCamera camera)
        {
            _camera = camera;
        }

        public void Update()
        {   
            _camera.Update();
        }
    }
}