using System;
using DG.Tweening;
using UnityEngine;

namespace dtank
{
    public class TitleCamera : MonoBehaviour, IDisposable
    {
        private Transform _transform;
        private Sequence _sequence;

        public void Setup()
        {
            _transform = transform;
        }

        public void Dispose()
        {
            _sequence?.Kill();
        }

        public void Play()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence()
                .Append(_transform.DOLocalRotate(new Vector3(0, 360f, 0), 12f, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear))
                .SetLoops(-1, LoopType.Restart)
                .SetLink(gameObject)
                .Play();
        }
    }
}