using System;
using UniRx;

namespace dtank
{
    public class NpcBehaviourObserver
    {
        public readonly IObserver<Unit> OnShotCurveObserver;
        public readonly IObserver<Unit> OnShotStraightObserver;
        public readonly IObserver<float> OnTurnValueChangedObserver;
        public readonly IObserver<float> OnMoveValueChangedObserver;

        public NpcBehaviourObserver(
            IObserver<Unit> onShotCurveObserver,
            IObserver<Unit> onShotStraightObserver,
            IObserver<float> onTurnValueChangedObserver,
            IObserver<float> onMoveValueChangedObserver)
        {
            OnShotCurveObserver = onShotCurveObserver;
            OnShotStraightObserver = onShotStraightObserver;
            OnTurnValueChangedObserver = onTurnValueChangedObserver;
            OnMoveValueChangedObserver = onMoveValueChangedObserver;
        }
    }
}