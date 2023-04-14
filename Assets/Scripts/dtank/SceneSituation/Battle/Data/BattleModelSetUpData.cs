using System.Collections.Generic;

namespace dtank
{
    public class BattleModelSetUpData
    {
        public BattleRuleData RuleData { get; private set; }
        public IReadOnlyDictionary<int, BattleTankModelSetupData> TankModelSetupDataDict => _tankModelSetupDataDict;

        private readonly Dictionary<int, BattleTankModelSetupData> _tankModelSetupDataDict;

        public BattleModelSetUpData(
            BattleRuleData ruleData,
            Dictionary<int, BattleTankModelSetupData> tankModelSetupDataDict)
        {
            RuleData = ruleData;
            _tankModelSetupDataDict = tankModelSetupDataDict;
        }
    }
}