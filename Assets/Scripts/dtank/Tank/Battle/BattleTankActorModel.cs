using System;
using GameFramework.ModelSystems;
using UniRx;

namespace dtank
{
    public class BattleTankActorModel : AutoIdModel<BattleTankActorModel>
    {
        public BattleTankActorSetupData Setup { get; private set; }
        public TransformData StartPointData { get; private set; }

        public event Action<BattleTankActorModel> OnUpdated;
        
        private BattleTankActorModel(int id) : base(id) {}

        public IObservable<BattleTankActorModel> OnUpdatedAsObservable() {
            return Observable.FromEvent<BattleTankActorModel>(
                h => OnUpdated += h,
                h => OnUpdated -= h);
        }

        public void Update(BattleTankActorSetupData setupData, TransformData startPointData) {
            Setup = setupData;
            StartPointData = startPointData;
            
            OnUpdated?.Invoke(this);
        }
    }
}