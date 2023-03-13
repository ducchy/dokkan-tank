namespace dtank
{
    public class NpcTankStateIdle : NpcTankStateBase
    {
        public override NpcTankState State => NpcTankState.Idle;
        private NpcTankStateResult _result = NpcTankStateResult.None;
        public override NpcTankStateResult Result => _result;

        private readonly float _duration;

        private float _elapsedTime;

        public NpcTankStateIdle(float duration)
        {
            _duration = duration;
        }

        public override void OnEnter()
        {
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
        }
    }
}