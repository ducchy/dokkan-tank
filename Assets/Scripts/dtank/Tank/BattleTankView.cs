namespace dtank
{
    public class BattleTankView : TankViewBase
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