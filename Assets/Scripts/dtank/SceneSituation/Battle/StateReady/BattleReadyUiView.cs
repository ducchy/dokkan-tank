using System;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;

namespace dtank
{
    public class BattleReadyUiView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private TextMeshProUGUI _countDownLabel;

        private readonly Subject<Unit> _onStart = new Subject<Unit>();
        public IObservable<Unit> OnStartObservable => _onStart;
        
        private Sequence _countDownSeq;		
        
        public void Construct()
        {
            Debug.Log("BattleReadyUiView.Construct()");

            SetActive(false);
        }

        private void OnDestroy()
        {
            _countDownSeq?.Kill();
        }

        private void SetActive(bool flag)
        {
            _group.alpha = flag ? 1f : 0f;
            _group.blocksRaycasts = flag;
        }
        
        public void PlayCountDown()
        {
            SetActive(true);
            SetCountDownSecondText(3);
            
            _countDownSeq?.Kill();
            _countDownSeq = DOTween.Sequence()
                .AppendInterval(1f)
                .AppendCallback(() => SetCountDownSecondText(2))
                .AppendInterval(1f)
                .AppendCallback(() => SetCountDownSecondText(1))
                .AppendInterval(1f)
                .AppendCallback(OnStart)
                .AppendInterval(1f)
                .OnComplete(() =>
                {
                    SetActive(false);
                })
                .SetLink(gameObject)
                .Play();
        }

        private void OnStart()
        {
            _onStart.OnNext(Unit.Default);
            SetCountDownStartText();
        }

        private void SetCountDownSecondText(int second)
        {
            _countDownLabel.text = second.ToString();
        }

        private void SetCountDownStartText()
        {
            _countDownLabel.text = "スタート！";
        }
    }
}