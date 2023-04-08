using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

namespace dtank
{
    public class BattlePlayerResultUiView : MonoBehaviour, IDisposable
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private RectTransform _base;
        [SerializeField] private TextMeshProUGUI _playerName;
        [SerializeField] private TextMeshProUGUI _rank;
        [SerializeField] private Image _rankBg;
        [SerializeField] private Image _arrow;

        private bool _openFlag;

        private Sequence _sequence;
        private Sequence _arrowSeq;

        public void Setup(string playerName, Color color, int rank, bool isMine)
        {
            _playerName.text = playerName;
            _rankBg.color = color;
            _rank.text = rank.ToString();
            _arrow.gameObject.SetActive(isMine);
            _arrow.color = color;
            
            SetActive(false);
        }

        public void Dispose()
        {
            _sequence?.Kill();
            _arrowSeq?.Kill();
        }

        public void SetActive(bool flag)
        {
            if (_group == null)
                return;

            _group.alpha = flag ? 1f : 0f;
            _group.blocksRaycasts = flag;

            var color = _arrow.color;
            color.a = flag ? 1f : 0f;
            _arrow.color = color;

            var pos = _base.anchoredPosition;
            pos.x = flag ? 0f : -400f;
            _base.anchoredPosition = pos;
        }

        public void Open()
        {
            if (_openFlag)
                return;

            _openFlag = true;

            _sequence?.Kill();
            _arrowSeq?.Kill();

            SetActive(false);

            _sequence = DOTween.Sequence()
                .Append(_group.DOFade(1f, 0.3f).SetEase(Ease.OutQuad))
                .Join(_base.DOAnchorPosX(0f, 0.3f).SetEase(Ease.OutQuad))
                .Append(_arrow.DOFade(0f, 0.15f).SetEase(Ease.OutQuad))
                .OnComplete(() =>
                {
                    SetActive(true);
                    _arrowSeq = DOTween.Sequence()
                        .Append(_arrow.rectTransform.DOAnchorPosX(20f, 0.3f))
                        .Append(_arrow.rectTransform.DOAnchorPosX(0f, 0.3f))
                        .SetLoops(-1, LoopType.Restart)
                        .SetLink(gameObject)
                        .Play();
                })
                .SetLink(gameObject)
                .Play();
        }

        public void Close()
        {
            if (!_openFlag)
                return;

            _openFlag = false;

            _sequence?.Kill();

            _sequence = DOTween.Sequence()
                .Append(_group.DOFade(0f, 0.3f))
                .OnComplete(() => SetActive(false))
                .SetLink(gameObject)
                .Play();
        }
    }
}