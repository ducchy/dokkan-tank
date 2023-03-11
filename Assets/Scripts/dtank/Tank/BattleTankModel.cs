using UniRx;

namespace dtank
{
    public class BattleTankModel : TankModelBase
    {
        private readonly TransformData _transformData;
        public ReactiveProperty<TransformData> TransformData => new ReactiveProperty<TransformData>(_transformData);

        public BattleTankModel(TransformData transformData)
        {
            _transformData = transformData;
        }
    }
}