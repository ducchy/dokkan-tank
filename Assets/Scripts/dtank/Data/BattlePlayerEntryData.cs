namespace dtank
{
    public class BattlePlayerEntryData
    {
        public readonly string Name;
        public readonly int ModelId;
        public readonly int PositionIndex;
        public CharacterType CharacterType;
        public readonly int ParameterId;

        public BattlePlayerEntryData(string name, int modelId, int positionIndex, CharacterType characterType, int parameterId)
        {
            Name = name;
            ModelId = modelId;
            PositionIndex = positionIndex;
            CharacterType = characterType;
            ParameterId = parameterId;
        }
    }
}