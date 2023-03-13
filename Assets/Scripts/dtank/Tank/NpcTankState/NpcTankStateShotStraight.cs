namespace dtank
{
    public class NpcTankStateShotStraight : NpcTankStateBase
    {
        public override NpcTankState State => NpcTankState.ShotStraight;
        public override NpcTankStateResult Result => NpcTankStateResult.None;

        private readonly NpcBehaviourSelector _behaviourSelector;

        public NpcTankStateShotStraight(NpcBehaviourSelector behaviourSelector)
        {
            _behaviourSelector = behaviourSelector;
        }

        public override void OnEnter()
        {
            _behaviourSelector.OnShotStraightListener?.Invoke();
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnExit()
        {
        }
    }
}