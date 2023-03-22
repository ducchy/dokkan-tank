using UnityEngine;

namespace dtank
{
    [CreateAssetMenu(fileName = "dat_tank_parameter_999.asset", menuName = "dtank/Battle/TankParameterData")]
    public class TankParameterData : ScriptableObject
    {
        public int hp;
        public float moveSpeed;
        public float turnSpeed;
        public float invincibleDuration;
    }
}