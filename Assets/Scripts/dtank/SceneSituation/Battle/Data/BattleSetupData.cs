namespace dtank
{
    public class BattleSetupData
    {
        public BattleModelSetupData ModelSetupData { get; private set; }
        public BattleActorSetupData ActorSetupData { get; private set; }

        public BattleSetupData(BattleModelSetupData modelSetupData, BattleActorSetupData actorSetupData)
        {
            ModelSetupData = modelSetupData;
            ActorSetupData = actorSetupData;
        }
    }
}