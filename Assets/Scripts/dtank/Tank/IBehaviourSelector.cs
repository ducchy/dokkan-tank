using System;

namespace dtank
{
    public interface IBehaviourSelector
    {
        Action OnDamageListener { set; }
        Action OnShotCurveListener { set; }
        Action OnShotStraightListener { set; }
        Action<float> OnTurnValueChangedListener { set; }
        Action<float> OnMoveValueChangedListener { set; }
    }
}