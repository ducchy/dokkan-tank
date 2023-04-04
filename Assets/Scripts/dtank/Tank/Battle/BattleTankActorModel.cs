using GameFramework.ModelSystems;

namespace dtank
{
    public class BattleTankActorModel : AutoIdModel<BattleTankActorModel>
    {
        public IBattleTankActorSetupData SetupData { get; private set; }
        public TransformData StartPointData { get; private set; }
        
        private BattleTankActorModel(int id) : base(id) {}

        public void Update(IBattleTankActorSetupData setupData, TransformData startPointData) {
            SetupData = setupData;
            StartPointData = startPointData;
        }
    }
}