using System;
using DG.Tweening;
using GameFramework.Core;
using GameFramework.EntitySystems;
using GameFramework.Kinematics;
using UniRx;
using UnityEngine;

namespace dtank
{
    public class BattleCameraController : MonoBehaviour, IDisposable
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _axis;
        [SerializeField] private ParentAttachment _parentAttachment;

        private BattleModel _model;
        private BattleTankEntityContainer _tankEntityContainer;

        private Sequence _readySeq;
        private Sequence _resultSeq;

        private readonly DisposableScope _scope = new DisposableScope();

        private readonly Subject<Unit> _onEndReadySubject = new Subject<Unit>();
        public IObservable<Unit> OnEndReadyAsObservable => _onEndReadySubject;

        public void Setup(BattleModel model, BattleTankEntityContainer tankEntityContainer)
        {
            _model = model;
            _tankEntityContainer = tankEntityContainer;
        }

        public void Dispose()
        {
            _readySeq?.Kill();
            _resultSeq?.Kill();

            _onEndReadySubject.Dispose();
        }

        private void SetTarget(int id)
        {
            var tankEntity = _tankEntityContainer.Get(id);
            if (tankEntity == null)
            {
                Debug.LogError($"注視対象取得失敗: id={id}");
                return;
            }
            _parentAttachment.Sources = new AttachmentResolver.TargetSource[]
                { new() { target = tankEntity.GetBody().Transform, weight = 1f } };
        }

        public void PlayReady()
        {
            _readySeq?.Kill();

            SetTarget(_model.MainPlayerTankModel.Id);

            _readySeq = DOTween.Sequence()
                .AppendInterval(1f)
                .Append(_axis.DOLocalRotate(new Vector3(0, 360f, 0), 4f, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear))
                .AppendInterval(1f)
                .OnComplete(() => _onEndReadySubject.OnNext(Unit.Default))
                .SetLink(gameObject)
                .Play();
        }

        public void SkipReady()
        {
            _readySeq?.Complete(true);
        }

        public void PlayResult(int winnerId)
        {
            _resultSeq?.Kill();

            SetTarget(winnerId);

            _resultSeq = DOTween.Sequence()
                .Append(_axis.DOLocalRotate(new Vector3(0, 360f, 0), 4f, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear))
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