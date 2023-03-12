namespace dtank
{
    public class BattleTankActor : TankActorBase
    {
        public void Construct()
        {
        }

        public void SetTransform(TransformData data)
        {
            transform.Set(data);
        }
    }
}