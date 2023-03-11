using System;
using DG.Tweening;
using TMPro;
using UniRx;
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
        
        public IObservable<Unit> OnRetryObservable => _retryButton.OnClickAsObservable();
        public IObservable<Unit> OnQuitObservable => _quitButton.OnClickAsObservable();

        private Sequence _resultSeq;
        
        public void Construct()
        {
            Debug.Log("BattleResultUiView.Construct()");

            SetActive(false);
        }

        private void OnDestroy()
        {
            _resultSeq?.Kill();
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
            SetActive(true);

            _resultLabel.text = winFlag ? "WIN!" : "LOSE!";
            
            _resultSeq?.Kill();
            _resultSeq = DOTween.Sequence()
                .Append(_group.DOFade(1f, 0.3f))
                .OnComplete(() =>
                {
                    SetActive(true);
                })
                .SetLink(gameObject)
                .Play();
        }
    }
}
