using UnityEngine;

namespace dtank
{
    public class NpcTankStateMove : NpcTankStateBase
    {
        public override NpcTankState State => NpcTankState.Move;
        private NpcTankStateResult _result = NpcTankStateResult.None;
        public override NpcTankStateResult Result => _result;

        private readonly NpcBehaviourSelector _controller;
        private readonly BattleTankModel _owner;
        private readonly float _duration;

        private float _elapsedTime;
        private float _stopBeginTime;
        private Vector3 _prePosition;

        public NpcTankStateMove(NpcBehaviourSelector controller, BattleTankModel owner, float duration)
        {
            _controller = controller;
            _owner = owner;
            _duration = duration;
        }

        public override void OnEnter()
        {
            _controller.OnMoveValueChangedListener?.Invoke(1f);
            _prePosition = Vector3.one * float.MaxValue;
            _elapsedTime = 0f;
            _stopBeginTime = float.MinValue;
        }

        public override void OnUpdate(float deltaTime)
        {
            _elapsedTime += deltaTime;
            if (_elapsedTime >= _duration)
                _result = NpcTankStateResult.Success;

            var currentPosition = _owner.Position;
            if (Vector3.Distance(_prePosition, currentPosition) <= 0.0001f)
            {
                if (_stopBeginTime < 0f)
                    _stopBeginTime = Time.realtimeSinceStartup;
                else if(Time.realtimeSinceStartup - _stopBeginTime > 0.1f)
                    _result = NpcTankStateResult.Failed;
            } else 
                _stopBeginTime = float.MinValue;

            _prePosition = currentPosition;
        }

        public override void OnExit()
        {
            _controller.OnMoveValueChangedListener?.Invoke(0f);
        }
    }
}