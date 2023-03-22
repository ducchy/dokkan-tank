using System.Collections.Generic;

namespace dtank
{
    public class BattleEntryData
    {
        public class User
        {
            public readonly int Id;
            public readonly int ModelId;
            public readonly int PositionIndex;
            public CharacterType CharacterType;
            public readonly int ParameterId;

            public User(int id, int modelId, int positionIndex, CharacterType characterType, int parameterId)
            {
                Id = id;
                ModelId = modelId;
                PositionIndex = positionIndex;
                CharacterType = characterType;
                ParameterId = parameterId;
            }
        }
        
        public int RuleId { get; private set; }
        public IReadOnlyList<User> Users => _users;
        
        private List<User> _users;

        public void Set(int ruleId, List<User> users)
        {
            RuleId = ruleId;
            _users = users;
        }
    }
}