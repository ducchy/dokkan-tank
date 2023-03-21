using System;
using UniRx;

namespace dtank
{
    public class NpcBehaviourObserver
    {
        public readonly IObserver<IAttacker> OnDamageObserver;
        public readonly IObserver<Unit> OnShotCurveObserver;
        public readonly IObserver<Unit> OnShotStraightObserver;
        public readonly IObserver<float> OnTurnValueChangedObserver;
        public readonly IObserver<float> OnMoveValueChangedObserver;

        public NpcBehaviourObserver(
            IObserver<IAttacker> onDamageObserver,
            IObserver<Unit> onShotCurveObserver,
            IObserver<Unit> onShotStraightObserver,
            IObserver<float> onTurnValueChangedObserver,
            IObserver<float> onMoveValueChangedObserver)
        {
            OnDamageObserver = onDamageObserver;
            OnShotCurveObserver = onShotCurveObserver;
            OnShotStraightObserver = onShotStraightObserver;
            OnTurnValueChangedObserver = onTurnValueChangedObserver;
            OnMoveValueChangedObserver = onMoveValueChangedObserver;
        }
    }
}