using System;
using UniRx;

namespace dtank
{
    public interface ITankBehaviour : IDisposable
    {
        IObservable<Unit> OnShotCurveAsObservable { get; }
        IObservable<Unit> OnShotStraightAsObservable { get; }
        IObservable<float> OnTurnValueChangedAsObservable { get; }
        IObservable<float> OnMoveValueChangedAsObservable { get; }

        void Reset();
        void SetActive(bool active);
        void BeginDamage();
        void EndDamage();
        void EndShotStraight();
        void Update();
    }
}