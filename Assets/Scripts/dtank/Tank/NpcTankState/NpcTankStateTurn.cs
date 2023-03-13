using UnityEngine;

namespace dtank
{
    public class NpcTankStateTurn : NpcTankStateBase
    {
        public override NpcTankState State => NpcTankState.Turn;
        private NpcTankStateResult _result = NpcTankStateResult.None;
        public override NpcTankStateResult Result => _result;

        private readonly NpcBehaviourSelector _controller;
        private readonly BattleTankModel _target;
        private readonly BattleTankModel _owner;
        private readonly float _limitDuration;

        private float _elapsedTime;

        public NpcTankStateTurn(NpcBehaviourSelector controller, BattleTankModel owner, BattleTankModel target,
            float limitDuration)
        {
            _controller = controller;
            _target = target;
            _owner = owner;
            _limitDuration = limitDuration;
        }

        public override void OnEnter()
        {
            _elapsedTime = 0f;
        }

        public override void OnUpdate(float deltaTime)
        {
            _elapsedTime += deltaTime;

            var angleToTarget = CalcAngleToTarget();
            var angleToTargetAbs = Mathf.Abs(angleToTarget);
            if (angleToTargetAbs < 0.05f)
            {
                _result = NpcTankStateResult.Success;
                return;
            }

            if (_elapsedTime >= _limitDuration)
            {
                _result = NpcTankStateResult.Failed;
                return;
            }

            var sign = Mathf.Sign(angleToTarget);
            var value = sign * Mathf.Clamp(angleToTargetAbs, 0f, 5f) / 5f;
            _controller.OnTurnValueChangedListener?.Invoke(value);
        }

        public override void OnExit()
        {
            _controller.OnTurnValueChangedListener?.Invoke(0f);
        }

        private float CalcAngleToTarget()
        {
            var diff = _target.Position - _owner.Position;
            var axis = Vector3.Cross(_owner.Forward, diff);
            return Vector3.Angle(_owner.Forward, diff) * (axis.y < 0 ? -1 : 1);
        }
    }
}