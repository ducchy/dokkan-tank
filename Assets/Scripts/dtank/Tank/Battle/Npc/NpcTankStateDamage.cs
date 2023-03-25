namespace dtank
{
    public class NpcTankStateDamage : NpcTankStateBase
    {
        public override NpcTankState State => NpcTankState.Damage;
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