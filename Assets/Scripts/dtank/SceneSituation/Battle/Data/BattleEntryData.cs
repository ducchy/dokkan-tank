using System;
using System.Collections.Generic;
using UnityEngine;

namespace dtank
{
    [Serializable]
    public class BattleEntryData
    {
        public int RuleId => _ruleId;
        public int MainPlayerId => _mainPlayerId;
        public IReadOnlyList<BattlePlayerEntryData> Players => _players;

        [SerializeField] private int _ruleId;
        [SerializeField] private int _mainPlayerId;
        [SerializeField] private List<BattlePlayerEntryData> _players;

        public BattleEntryData(int ruleId, int mainPlayerId, List<BattlePlayerEntryData> players)
        {
            _ruleId = ruleId;
            _mainPlayerId = mainPlayerId;
            _players = players;
        }

        public static BattleEntryData CreateDefaultData()
        {
            return new(1, 1, new List<BattlePlayerEntryData>()
            {
                new(1, "プレイヤー1", 1, 0, CharacterType.Player, 1),
                new(2, "プレイヤー2", 1, 1, CharacterType.NonPlayer, 2),
                new(3, "プレイヤー3", 1, 2, CharacterType.NonPlayer, 3),
                new(4, "プレイヤー4", 1, 3, CharacterType.NonPlayer, 4),
            });
        }
    }
}