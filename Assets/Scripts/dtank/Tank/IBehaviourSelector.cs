using System;

namespace dtank
{
    public interface IBehaviourSelector : IDisposable
    {
        Action<IAttacker> OnDamageListener { set; }
        Action OnShotCurveListener { set; }
        Action OnShotStraightListener { set; }
        Action<float> OnTurnValueChangedListener { set; }
        Action<float> OnMoveValueChangedListener { set; }

        void Reset();
        void SetActive(bool active);
        void BeginDamage();
        void EndDamage();
        void EndShotStraight();
    }
}