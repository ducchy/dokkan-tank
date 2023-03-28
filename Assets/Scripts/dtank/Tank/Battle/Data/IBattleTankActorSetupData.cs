using UnityEditor.Animations;
using UnityEngine;

namespace dtank
{
    public interface IBattleTankActorSetupData
    {
        float MoveMaxSpeed { get; }
        float TurnMaxSpeed { get; }
        AnimatorController Controller { get; }
        float ShellSpeedOnShotCurve { get; }
        float ShellSpeedOnShotStraight { get; }
        ShellActor ShellPrefab { get; }
        GameObject DeadEffectPrefab { get; }
        GameObject FireEffectPrefab { get; }
    }
}