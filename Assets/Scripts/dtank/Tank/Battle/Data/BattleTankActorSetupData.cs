using UnityEngine;

namespace dtank
{
    [CreateAssetMenu(fileName = "dat_tank_actor_setup_999.asset", menuName = "dtank/Battle/TankActorSetupData")]
    public class BattleTankActorSetupData : ScriptableObject
    {
        public float moveMaxSpeed;
        public float turnMaxSpeed;
    }
}