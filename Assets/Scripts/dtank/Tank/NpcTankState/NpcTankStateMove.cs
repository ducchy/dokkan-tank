namespace dtank
{
    public class NpcTankStateMove : NpcTankStateBase
    {
        public override NpcTankState State => NpcTankState.Move;
        private NpcTankStateResult _result = NpcTankStateResult.None;
        public override NpcTankStateResult Result => _result;

        private readonly NpcBehaviourSelector _controller;
        private readonly float _duration;

        private float _elapsedTime;

        public NpcTankStateMove(NpcBehaviourSelector controller, float duration)
        {
            _controller = controller;
            _duration = duration;
        }

        public override void OnEnter()
        {
            _controller.OnMoveValueChangedListener?.Invoke(1f);
            _elapsedTime = 0f;
        }

        public override void OnUpdate(float deltaTime)
        {
            _elapsedTime += deltaTime;
            if (_elapsedTime >= _duration)
                _result = NpcTankStateResult.Success;
        }

        public override void OnExit()
        {
            _controller.OnMoveValueChangedListener?.Invoke(0f);
        }
    }
}