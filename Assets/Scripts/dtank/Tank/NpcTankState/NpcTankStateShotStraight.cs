using UniRx;

namespace dtank
{
    public class NpcTankStateShotStraight : NpcTankStateBase
    {
        public override NpcTankState State => NpcTankState.ShotStraight;
        public override NpcTankStateResult Result => NpcTankStateResult.None;

        private readonly NpcBehaviourObserver _observer;

        public NpcTankStateShotStraight(NpcBehaviourObserver observer)
        {
            _observer = observer;
        }

        public override void OnEnter()
        {
            _observer.OnShotStraightObserver.OnNext(Unit.Default);
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnExit()
        {
        }
    }
}