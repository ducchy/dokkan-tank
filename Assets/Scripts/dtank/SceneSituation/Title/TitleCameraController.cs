using System;
using DG.Tweening;
using UnityEngine;

namespace dtank
{
    public class TitleCameraController : MonoBehaviour, IDisposable
    {
        // [SerializeField] private Camera _camera;
        [SerializeField] private Transform _axis;
        private Sequence _sequence;

        public void Dispose()
        {
            _sequence?.Kill();
        }

        public void Play()
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence()
                .Append(_axis.DOLocalRotate(new Vector3(0, 360f, 0), 12f, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear))
                .SetLoops(-1, LoopType.Restart)
                .SetLink(gameObject)
                .Play();
        }
    }
}