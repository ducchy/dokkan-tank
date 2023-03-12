namespace dtank
{
    public class BattleTankActorFactory : TankActorFactory<BattleTankActor>
    {
        protected override string CreatePrefabPath(int id)
        {
            return $"Prefab/BattleTank_{id:d3}";
        }

        protected override void OnCreated(BattleTankActor actor)
        {
            actor.Construct();
        }
    }
}