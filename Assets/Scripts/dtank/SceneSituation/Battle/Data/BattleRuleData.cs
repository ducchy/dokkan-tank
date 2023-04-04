using UnityEngine;

namespace dtank
{
    [CreateAssetMenu(fileName = "dat_rule_999.asset", menuName = "dtank/Battle/BattleRuleData")]
    public class BattleRuleData : ScriptableObject
    {
        [SerializeField] private float _duration;
        
        public float Duration => _duration;
    }
}