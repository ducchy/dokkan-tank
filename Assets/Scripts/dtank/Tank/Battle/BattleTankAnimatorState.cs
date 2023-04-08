namespace dtank
{
    public enum BattleTankAnimatorState
    {
        Invalid,
        Idle,
        Damage,
        ShotCurve,
        ShotStraight,
        Dead,
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
                case BattleTankAnimatorState.Dead:
                    return @this.ToString();
                default: return string.Empty;
            }
        }
    }
}