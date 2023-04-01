using UnityEditor.Animations;
using UnityEngine;

namespace dtank
{
    public interface IBattleTankActorSetupData
    {
        float MoveMaxSpeed { get; }
        float TurnMaxSpeed { get; }
        AnimatorController Controller { get; }
        GameObject DeadEffectPrefab { get; }
        ActionInfo[] ActionInfos { get; }
    }
}