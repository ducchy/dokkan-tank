namespace dtank
{
    public class BattleTankModelSetupData
    {
        public BattleTankParameterData ParameterData { get; private set; }
        public BattleTankActorSetupData ActorSetupData { get; private set; }

        public BattleTankModelSetupData(
            BattleTankParameterData parameterData, 
            BattleTankActorSetupData actorSetupData)
        {
            ParameterData = parameterData;
            ActorSetupData = actorSetupData;
        }
    }
}