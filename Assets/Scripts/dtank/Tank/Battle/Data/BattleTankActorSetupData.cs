using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Serialization;

namespace dtank
{
    [CreateAssetMenu(fileName = "dat_tank_actor_setup_999.asset", menuName = "dtank/Battle/TankActorSetupData")]
    public class BattleTankActorSetupData : ScriptableObject, IBattleTankActorSetupData
    {
        public float moveMaxSpeed;
        public float turnMaxSpeed;
        public AnimatorController controller;
        public GameObject _deadEffectPrefab;
        public ActionInfo[] _actionInfos;

        float IBattleTankActorSetupData.MoveMaxSpeed => moveMaxSpeed;
        float IBattleTankActorSetupData.TurnMaxSpeed => turnMaxSpeed;
        AnimatorController IBattleTankActorSetupData.Controller => controller;
        GameObject IBattleTankActorSetupData.DeadEffectPrefab => _deadEffectPrefab;
        ActionInfo[] IBattleTankActorSetupData.ActionInfos => _actionInfos;
    }
}