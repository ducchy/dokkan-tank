using UnityEditor.Animations;
using UnityEngine;

namespace dtank
{
    [CreateAssetMenu(fileName = "dat_tank_actor_setup_999.asset", menuName = "dtank/Battle/TankActorSetupData")]
    public class BattleTankActorSetupData : ScriptableObject, IBattleTankActorSetupData
    {
        [SerializeField] private float _moveMaxSpeed;
        [SerializeField] private float _turnMaxSpeed;
        [SerializeField] private AnimatorController _controller;
        [SerializeField] private ActionInfo[] _actionInfos;
        [SerializeField] private Color _color;

        private TransformData _startPointData;

        float IBattleTankActorSetupData.MoveMaxSpeed => _moveMaxSpeed;
        float IBattleTankActorSetupData.TurnMaxSpeed => _turnMaxSpeed;
        AnimatorController IBattleTankActorSetupData.Controller => _controller;
        ActionInfo[] IBattleTankActorSetupData.ActionInfos => _actionInfos;
        Color IBattleTankActorSetupData.Color => _color;
        TransformData IBattleTankActorSetupData.StartPointData => _startPointData;

        public void SetNonSerializeData(TransformData startPointData)
        {
            _startPointData = startPointData;
        }
    }
}