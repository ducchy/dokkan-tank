namespace dtank
{
    public class BattleTankActorFactory : TankActorFactoryBase<BattleTankActor>
    {
        protected override string CreatePrefabPath(int id)
        {
            return $"Prefab/BattleTank_{id:d3}";
        }
    }
}