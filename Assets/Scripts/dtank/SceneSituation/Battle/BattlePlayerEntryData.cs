namespace dtank
{
    public class BattlePlayerEntryData
    {
        public readonly int PlayerId;
        public readonly string Name;
        public readonly int BodyId;
        public readonly int PositionIndex;
        public readonly CharacterType CharacterType;
        public readonly int ParameterId;

        public BattlePlayerEntryData(int playerId, string name, int bodyId, int positionIndex, CharacterType characterType, int parameterId)
        {
            PlayerId = playerId;
            Name = name;
            BodyId = bodyId;
            PositionIndex = positionIndex;
            CharacterType = characterType;
            ParameterId = parameterId;
        }
    }
}