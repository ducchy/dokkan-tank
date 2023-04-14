using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace dtank
{
    public class BattleReadyUiView : MonoBehaviour, IDisposable
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Button _skipButton;

        public IObservable<Unit> OnSkipButtonClickObservable => _skipButton.OnClickAsObservable();

        private Sequence _seq;

        public void Dispose()
        {
            _seq?.Kill();
        }

        public void Reset()
        {
            SetActive(false);
        }

        private void SetActive(bool flag)
        {
            if (_group == null)
                return;
            
            _group.alpha = flag ? 1f : 0f;
            _group.blocksRaycasts = flag;
        }

        public void Begin()
        {
            SetActive(false);

            _seq?.Kill();
            _seq = DOTween.Sequence()
                .AppendInterval(0.5f)
                .Append(_group.DOFade(1f, 0.3f))
                .OnComplete(() => SetActive(true))
                .SetLink(gameObject)
                .Play();
        }

        public void End()
        {
            SetActive(true);

            if (_group == null)
                return;

            _seq?.Kill();
            _seq = DOTween.Sequence()
                .Append(_group.DOFade(0f, 0.3f))
                .OnComplete(() => SetActive(false))
                .SetLink(gameObject)
                .Play();
        }
    }
}