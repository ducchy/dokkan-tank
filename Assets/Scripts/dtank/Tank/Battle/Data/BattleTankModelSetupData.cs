namespace dtank
{
    public class BattleTankModelSetupData
    {
        public BattleTankParameterData ParameterData { get; private set; }

        public BattleTankModelSetupData(
            BattleTankParameterData parameterData)
        {
            ParameterData = parameterData;
        }
    }
}