using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace dtank
{
    public class BattleReadyUiView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Button _skipButton;

        public Action OnSkipButtonClickedListener;

        private Sequence _seq;

        public void Construct()
        {
            Debug.Log("BattleReadyUiView.Construct()");

            _skipButton.onClick.AddListener(OnSkipButtonClicked);
            
            SetActive(false);
        }

        private void OnDestroy()
        {
            _seq?.Kill();
            
            _skipButton.onClick.RemoveListener(OnSkipButtonClicked);
        }

        private void SetActive(bool flag)
        {
            if (_group == null)
                return;
            
            _group.alpha = flag ? 1f : 0f;
            _group.blocksRaycasts = flag;
        }

        public void BeginReady()
        {
            SetActive(false);

            _seq?.Kill();
            _seq = DOTween.Sequence()
                .Append(_group.DOFade(1f, 0.3f))
                .OnComplete(() => SetActive(true))
                .SetLink(gameObject)
                .Play();
        }

        public void EndReady()
        {
            SetActive(true);

            _seq?.Kill();
            _seq = DOTween.Sequence()
                .Append(_group.DOFade(0f, 0.3f))
                .OnComplete(() => SetActive(false))
                .SetLink(gameObject)
                .Play();
        }

        private void OnSkipButtonClicked()
        {
            OnSkipButtonClickedListener?.Invoke();
        }
    }
}