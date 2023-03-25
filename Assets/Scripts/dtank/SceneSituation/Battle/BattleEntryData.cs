using System.Collections.Generic;

namespace dtank
{
    public class BattleEntryData
    {
        public int RuleId { get; private set; }
        public IReadOnlyList<BattlePlayerEntryData> Players => _players;
        
        private List<BattlePlayerEntryData> _players;

        public void Set(int ruleId, List<BattlePlayerEntryData> players)
        {
            RuleId = ruleId;
            _players = players;
        }
    }
}