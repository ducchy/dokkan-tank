namespace dtank
{
    public class BattleTankController
    {
        private readonly BattleTankModel _model;
        private readonly BattleTankActor _actor;
        
        public BattleTankController(
            BattleTankModel model,
            BattleTankActor actor)
        {
            _model = model;
            _actor = actor;
        }

        public void SetStartPoint()
        {
           //  _actor.SetTransform(_model.StartPointData);
        }
    }
}