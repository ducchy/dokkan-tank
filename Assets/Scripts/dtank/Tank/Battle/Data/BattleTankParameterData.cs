using UnityEngine;

namespace dtank
{
    [CreateAssetMenu(fileName = "dat_tank_parameter_999.asset", menuName = "dtank/Battle/TankParameterData")]
    public class BattleTankParameterData : ScriptableObject
    {
        public int hp;
        public float invincibleDuration;
        public int actorSetupDataId;
    }
}