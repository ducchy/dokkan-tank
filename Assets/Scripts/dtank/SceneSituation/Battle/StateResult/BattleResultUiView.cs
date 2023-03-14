using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace dtank
{
    public class BattleResultUiView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private TextMeshProUGUI _resultLabel;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private RectTransform _retryButtonRect;
        [SerializeField] private RectTransform _quitButtonRect;

        public Action OnRetryButtonClickedListener;
        public Action OnQuitButtonClickedListener;

        private Sequence _resultSeq;
        
        public void Construct()
        {
            Debug.Log("BattleResultUiView.Construct()");
            
            _retryButton.onClick.AddListener(OnRetryButtonClicked);
            _quitButton.onClick.AddListener(OnQuitButtonClicked);

            SetActive(false);
        }

        private void OnDestroy()
        {
            _resultSeq?.Kill();
            
            _retryButton.onClick.RemoveListener(OnRetryButtonClicked);
            _quitButton.onClick.RemoveListener(OnQuitButtonClicked);
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
            SetActive(false);

            _resultLabel.text = winFlag ? "WIN!" : "LOSE!";
            _resultLabel.color = new Color(0.2f, 0.2f, 0.2f, 0f);
            _resultLabel.transform.localScale = Vector3.one * 3f;

            _retryButtonRect.anchoredPosition = new Vector2(200f, 200f);
            _quitButtonRect.anchoredPosition = new Vector2(200f, 500f);
            
            _resultSeq?.Kill();
            _resultSeq = DOTween.Sequence()
                .Append(_group.DOFade(1f, 0.3f))
                .Append(_resultLabel.DOFade(1f, 0.3f).SetEase(Ease.OutQuad))
                .Join(_resultLabel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutQuad))
                .AppendInterval(1f)
                .Append(_retryButtonRect.DOAnchorPosX(-200f, 0.3f).SetEase(Ease.OutQuad))
                .Append(_quitButtonRect.DOAnchorPosX(-200f, 0.3f).SetEase(Ease.OutQuad))
                .OnComplete(() =>
                {
                    SetActive(true);
                })
                .SetLink(gameObject)
                .Play();
        }

        private void OnRetryButtonClicked()
        {
            OnRetryButtonClickedListener?.Invoke();
        }

        private void OnQuitButtonClicked()
        {
            OnQuitButtonClickedListener?.Invoke();
        }
    }
}
