using System;
using DG.Tweening;
using GameFramework.Core;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace dtank
{
    public class BattleResultUiView : MonoBehaviour, IDisposable
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private TextMeshProUGUI _resultLabel;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private RectTransform _retryButtonRect;
        [SerializeField] private RectTransform _quitButtonRect;

        public IObservable<Unit> OnRetryButtonClickedAsObservable => _retryButton.OnClickAsObservable();
        public IObservable<Unit> OnQuitButtonClickedAsObservable => _quitButton.OnClickAsObservable();

        private Sequence _resultSeq;
        private FadeController _fadeController;
        private readonly DisposableScope _fadeScope = new();

        public void Setup(FadeController fadeController)
        {
            _fadeController = fadeController;
        }

        public void Dispose()
        {
            _resultSeq?.Kill();

            _fadeScope.Dispose();
        }

        public void Reset()
        {
            SetActive(false);
        }

        public void SetActive(bool flag)
        {
            if (_group == null)
                return;

            _group.alpha = flag ? 1f : 0f;
            _group.blocksRaycasts = flag;
        }

        public void PlayResult(bool winFlag)
        {
            _fadeScope.Dispose();

            SetActive(false);

            void onCompleteFadeIn()
            {
                _resultSeq?.Kill();
                
                _resultLabel.text = winFlag ? "WIN!" : "LOSE!";
                _resultLabel.color = new Color(0.2f, 0.2f, 0.2f, 0f);
                _resultLabel.transform.localScale = Vector3.one * 3f;

                _retryButtonRect.anchoredPosition = new Vector2(200f, 200f);
                _quitButtonRect.anchoredPosition = new Vector2(200f, 500f);

                _resultSeq = DOTween.Sequence()
                    .Append(_group.DOFade(1f, 0.3f))
                    .Append(_resultLabel.DOFade(1f, 0.3f).SetEase(Ease.OutQuad))
                    .Join(_resultLabel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutQuad))
                    .AppendInterval(1f)
                    .Append(_retryButtonRect.DOAnchorPosX(-200f, 0.3f).SetEase(Ease.OutQuad))
                    .Append(_quitButtonRect.DOAnchorPosX(-200f, 0.3f).SetEase(Ease.OutQuad))
                    .OnComplete(() => { SetActive(true); })
                    .SetLink(gameObject)
                    .Play();
            }

            _fadeController.FadeIn(0.5f, onCompleteFadeIn, _fadeScope);
        }
    }
}