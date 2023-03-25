namespace dtank
{
    public abstract class NpcTankStateBase
    {
        public abstract NpcTankState State { get; }
        public abstract NpcTankStateResult Result { get; }

        public abstract void OnEnter();
        public abstract void OnUpdate(float deltaTime);
        public abstract void OnExit();
    }
}