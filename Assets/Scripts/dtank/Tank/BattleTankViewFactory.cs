namespace dtank
{
    public class BattleTankViewFactory : TankViewFactory<BattleTankView>
    {
        protected override string CreatePrefabPath(int id)
        {
            return $"Prefab/BattleTank_{id:d3}";
        }

        protected override void OnCreated(BattleTankView view)
        {
            view.Construct();
        }
    }
}