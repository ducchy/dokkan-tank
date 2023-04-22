using GameFramework.ModelSystems;

namespace dtank
{
    public class BattleTankActorModel : AutoIdModel<BattleTankActorModel>
    {
        public IBattleTankActorSetupData SetupData { get; private set; }
        
        private BattleTankActorModel(int id) : base(id) {}

        public void Update(IBattleTankActorSetupData setupData) {
            SetupData = setupData;
        }
    }
}