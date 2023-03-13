namespace dtank
{
    public enum BattleResultType
    {
        None, 
        Win, 
        Lose,
    }
    
    public class BattleResultData
    {
        public BattleResultType ResultType { get; private set; }
        
        public void SetResultType(BattleResultType resultType)
        {
            ResultType = resultType;
        }
    }
}