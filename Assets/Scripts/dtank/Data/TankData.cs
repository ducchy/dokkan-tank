namespace dtank
{
    public enum CharacterType
    {
        Player,
        NonPlayer, 
    }
    
    public class TankData
    {
        public int OwnerId { get; private set; }
        public int ModelId { get; private set; }
        public int PositionIndex { get; private set; }
        public CharacterType CharacterType { get; private set; }
        public float InvincibleDuration { get; private set; }

        public TankData(int ownerId, int modelId, int positionIndex, CharacterType characterType, float invincibleDuration)
        {
            this.OwnerId = ownerId;
            this.ModelId = modelId;
            this.PositionIndex = positionIndex;
            this.CharacterType = characterType;
            this.InvincibleDuration = invincibleDuration;
        }
    }
}