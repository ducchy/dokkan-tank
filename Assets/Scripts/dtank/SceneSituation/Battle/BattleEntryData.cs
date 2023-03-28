using System.Collections.Generic;

namespace dtank
{
    public class BattleEntryData
    {
        public int RuleId { get; private set; }
        public BattlePlayerEntryData MainPlayer { get; private set; }
        public IReadOnlyList<BattlePlayerEntryData> Players => _players;
        
        private List<BattlePlayerEntryData> _players;

        public void Set(int ruleId, BattlePlayerEntryData mainPlayer, List<BattlePlayerEntryData> players)
        {
            RuleId = ruleId;
            MainPlayer = mainPlayer;
            _players = players;
        }
    }
}