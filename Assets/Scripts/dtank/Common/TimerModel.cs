using System;
using UniRx;
using UnityEngine;

namespace dtank
{
    public class TimerModel : IDisposable
    {
        private readonly ReactiveProperty<int> _remainTimeInt = new();
        public IReadOnlyReactiveProperty<int> RemainTime => _remainTimeInt;

        private readonly ReactiveProperty<bool> _activeFlag = new();
        public IReadOnlyReactiveProperty<bool> ActiveFlag => _activeFlag;

        private readonly Subject<Unit> _timeUpSubject = new();
        public IObservable<Unit> TimeUpObservable => _timeUpSubject;

        public readonly float InitTime;
        private float _remainTime;

        public TimerModel(float initTime)
        {
            InitTime = initTime;

            Reset();
        }

        public void Dispose()
        {
            _remainTimeInt.Dispose();
            _activeFlag.Dispose();
            _timeUpSubject.Dispose();
        }

        public void SetActive(bool active)
        {
            _activeFlag.Value = active;
        }

        public void Reset()
        {
            SetTime(InitTime);
        }

        private void SetTime(float time)
        {
            _remainTime = time;
            _remainTimeInt.Value = Mathf.Max(0, Mathf.CeilToInt(_remainTime));
        }

        public void Update(float deltaTime)
        {
            if (!_activeFlag.Value)
                return;

            SetTime(_remainTime - deltaTime);

            if (_remainTime <= 0f)
                TimeUp();
        }

        private void TimeUp()
        {
            _timeUpSubject.OnNext(Unit.Default);
            _activeFlag.Value = false;
        }
    }
}