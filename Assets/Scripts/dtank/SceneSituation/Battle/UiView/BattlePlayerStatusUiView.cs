using System;
using DG.Tweening;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace dtank
{
    public class BattlePlayerStatusUiView : MonoBehaviour, IDisposable
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private HpGaugeView _hpGauge;

        private bool _openFlag;
        
        private Sequence _sequence;

        public void Dispose()
        {
            _sequence?.Kill();
        }

        public void Reset()
        {
            _openFlag = false;
            SetActive(false);
            
            _hpGauge.SetHpImmediate(0);
        }

        private void SetActive(bool flag)
        {
            if (_group == null)
                return;
            
            _group.alpha = flag ? 1f : 0f;
            _group.blocksRaycasts = flag;
        }

        public void SetHp(int hp)
        {
            _hpGauge.SetHp(hp);
            
            if (_openFlag)
                _hpGauge.Play();
        }

        public void Open()
        {
            if (_openFlag)
                return;
            
            Debug.Log("[BattlePlayerStatusUiView] Open");

            _openFlag = true;
            
            _sequence?.Kill();

            SetActive(false);

            _sequence = DOTween.Sequence()
                .Append(_group.DOFade(1f, 0.3f))
                .OnComplete(() =>
                {
                    _hpGauge.Play(() => SetActive(true));
                })
                .SetLink(gameObject)
                .Play();
        }

        public void Close()
        {
            if (!_openFlag)
                return;
            
            Debug.Log("[BattlePlayerStatusUiView] Close");

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