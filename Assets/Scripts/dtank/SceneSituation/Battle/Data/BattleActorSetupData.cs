using System.Collections.Generic;

namespace dtank
{
    public class BattleActorSetupData
    {
        public IReadOnlyDictionary<int, BattleTankActorSetupData> TankActorSetupDataDict => _tankActorSetupDataDict;

        private readonly Dictionary<int, BattleTankActorSetupData> _tankActorSetupDataDict;

        public BattleActorSetupData(
            Dictionary<int, BattleTankActorSetupData> tankActorSetupDataDict)
        {
            _tankActorSetupDataDict = tankActorSetupDataDict;
        }
    }
}