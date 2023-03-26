namespace dtank
{
    public class BattlePlayerEntryData
    {
        public readonly string Name;
        public readonly int BodyId;
        public readonly int PositionIndex;
        public readonly CharacterType CharacterType;
        public readonly int ParameterId;

        public BattlePlayerEntryData(string name, int bodyId, int positionIndex, CharacterType characterType, int parameterId)
        {
            Name = name;
            BodyId = bodyId;
            PositionIndex = positionIndex;
            CharacterType = characterType;
            ParameterId = parameterId;
        }
    }
}