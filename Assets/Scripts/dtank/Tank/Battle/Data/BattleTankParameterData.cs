using UnityEngine;

namespace dtank
{
    [CreateAssetMenu(fileName = "dat_tank_parameter_999.asset", menuName = "dtank/Battle/TankParameterData")]
    public class BattleTankParameterData : ScriptableObject
    {
        [SerializeField] private int _hp;
        [SerializeField] private float _invincibleDuration;
        [SerializeField] private int _actorSetupDataId;

        public int Hp => _hp;
        public float InvincibleDuration => _invincibleDuration;
        public int ActorSetupDataId => _actorSetupDataId;
    }
}