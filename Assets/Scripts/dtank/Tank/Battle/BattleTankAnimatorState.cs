namespace dtank
{
    public enum BattleTankAnimatorState
    {
        Invalid,
        Idle,
        Damage,
        ShotCurve,
        ShotStraight,
    }

    public static class BattleTankAnimatorStateExtension
    {
        public static string ToStateName(this BattleTankAnimatorState @this)
        {
            switch (@this)
            {
                case BattleTankAnimatorState.Idle:
                case BattleTankAnimatorState.Damage:
                case BattleTankAnimatorState.ShotCurve:
                case BattleTankAnimatorState.ShotStraight:
                    return @this.ToString();
                default: return string.Empty;
            }
        }
    }
}