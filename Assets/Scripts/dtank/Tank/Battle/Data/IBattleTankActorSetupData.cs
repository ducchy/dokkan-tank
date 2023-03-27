using UnityEditor.Animations;

namespace dtank
{
    public interface IBattleTankActorSetupData
    {
        float MoveMaxSpeed { get; }
        float TurnMaxSpeed { get; }
        AnimatorController Controller { get; }
        float ShellSpeedOnShotCurve  { get; }
        float ShellSpeedOnShotStraight { get; }
        ShellActor ShellPrefab { get; }
    }
}