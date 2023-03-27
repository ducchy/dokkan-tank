using UnityEditor.Animations;
using UnityEngine;

namespace dtank
{
    [CreateAssetMenu(fileName = "dat_tank_actor_setup_999.asset", menuName = "dtank/Battle/TankActorSetupData")]
    public class BattleTankActorSetupData : ScriptableObject, IBattleTankActorSetupData
    {
        public float moveMaxSpeed;
        public float turnMaxSpeed;
        public AnimatorController controller;
        public float _shellSpeedOnShotCurve;
        public float _shellSpeedOnShotStraight;
        public ShellActor _shellPrefab;

        float IBattleTankActorSetupData.MoveMaxSpeed => moveMaxSpeed;
        float IBattleTankActorSetupData.TurnMaxSpeed => turnMaxSpeed;
        AnimatorController IBattleTankActorSetupData.Controller => controller;
        float IBattleTankActorSetupData.ShellSpeedOnShotCurve => _shellSpeedOnShotCurve;
        float IBattleTankActorSetupData.ShellSpeedOnShotStraight => _shellSpeedOnShotStraight;
        ShellActor IBattleTankActorSetupData.ShellPrefab => _shellPrefab;
    }
}