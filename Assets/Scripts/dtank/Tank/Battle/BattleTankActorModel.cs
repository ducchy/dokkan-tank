using GameFramework.ModelSystems;

namespace dtank
{
    public class BattleTankActorModel : AutoIdModel<BattleTankActorModel>
    {
        public BattleTankActorSetupData Setup { get; private set; }
        public TransformData StartPointData { get; private set; }
        
        private BattleTankActorModel(int id) : base(id) {}

        public void Update(BattleTankActorSetupData setupData, TransformData startPointData) {
            Setup = setupData;
            StartPointData = startPointData;
        }
    }
}