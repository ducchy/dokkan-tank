using System.Collections.Generic;

namespace dtank
{
    public class BattleModelSetupData
    {
        public BattleRuleData RuleData { get; private set; }
        public IReadOnlyDictionary<int, BattleTankModelSetupData> TankModelSetupDataDict => _tankModelSetupDataDict;

        private readonly Dictionary<int, BattleTankModelSetupData> _tankModelSetupDataDict;

        public BattleModelSetupData(
            BattleRuleData ruleData,
            Dictionary<int, BattleTankModelSetupData> tankModelSetupDataDict)
        {
            RuleData = ruleData;
            _tankModelSetupDataDict = tankModelSetupDataDict;
        }
    }
}