using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace dtank
{
    public class BattleCamera : MonoBehaviour, IDisposable
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Vector3 _offsetPos = new Vector3(0f, 1.5f, -3.5f);
        [SerializeField] private Vector3 _offsetAngle = new Vector3(15f, 0f, 0f);

        private Transform _transform;
        private Transform _cameraTransform;
        private Sequence _readySeq;
        private Sequence _resultSeq;

        private readonly Subject<Unit> _onEndReadySubject = new Subject<Unit>();
        public IObservable<Unit> OnEndReadyAsObservable => _onEndReadySubject;

        public void Dispose()
        {
            _readySeq?.Kill();
            _resultSeq?.Kill();
            
            _onEndReadySubject.Dispose();
        }

        public void SetFollowTarget(Transform target)
        {
            _readySeq?.Kill();
            _resultSeq?.Kill();
            
            _transform = transform;
            _cameraTransform = _camera.transform;
            
            _transform.SetParent(target);
            _transform.localPosition = Vector3.zero;
            _transform.localEulerAngles = Vector3.zero;
            SetOffsetPos(_offsetPos);
            SetOffsetAngle(_offsetAngle);
        }

        private void SetOffsetPos(Vector3 offset)
        {
            _cameraTransform.localPosition = offset;
        }

        private void SetOffsetAngle(Vector3 offset)
        {
            _cameraTransform.localEulerAngles = offset;
        }

        public void PlayReady()
        {
            _readySeq?.Kill();

            _readySeq = DOTween.Sequence()
                .AppendInterval(1f)
                .Append(_transform.DOLocalRotate(new Vector3(0, 360f, 0), 4f, RotateMode.FastBeyond360).SetEase(Ease.Linear))
                .AppendInterval(1f)
                .OnComplete(() => _onEndReadySubject.OnNext(Unit.Default))
                .SetLink(gameObject)
                .Play();
        }

        public void SkipReady()
        {
            _readySeq?.Complete(true);
        }

        public void PlayResult()
        {
            _resultSeq?.Kill();

            _resultSeq = DOTween.Sequence()
                .Append(_transform.DOLocalRotate(new Vector3(0, 360f, 0), 4f, RotateMode.FastBeyond360).SetEase(Ease.Linear))
                .SetLoops(-1, LoopType.Restart)
                .SetLink(gameObject)
                .Play();
        }

        public void EndResult()
        {
            _resultSeq?.Kill();
        }
    }
}