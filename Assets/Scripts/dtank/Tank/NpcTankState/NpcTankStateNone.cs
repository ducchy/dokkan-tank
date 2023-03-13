namespace dtank
{
    public class NpcTankStateNone : NpcTankStateBase
    {
        public override NpcTankState State => NpcTankState.None;
        public override NpcTankStateResult Result => NpcTankStateResult.None;

        public override void OnEnter()
        {
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnExit()
        {
        }
    }
}