using UnityEngine;

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
                case BattleTankAnimatorState.Idle: return "idle";
                case BattleTankAnimatorState.Damage: return "damage";
                case BattleTankAnimatorState.ShotCurve: return "shot_curve";
                case BattleTankAnimatorState.ShotStraight: return "shot_straight";
                default: return string.Empty;
            }
        }

        public static int ToStateHash(this BattleTankAnimatorState @this)
        {
            return Animator.StringToHash(@this.ToStateName());
        }
    }
}