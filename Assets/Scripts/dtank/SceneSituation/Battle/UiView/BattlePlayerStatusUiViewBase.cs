using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace dtank
{
    public abstract class BattlePlayerStatusUiViewBase : MonoBehaviour, IBattlePlayerStatusUiView, IDisposable
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private TextMeshProUGUI _playerName;
        [SerializeField] private TextMeshProUGUI _score;
        [SerializeField] private HpGaugeView _hpGauge;

        private bool _openFlag;

        private Sequence _sequence;
        private Sequence _scoreSeq;

        public void Setup(string playerName, int maxHp)
        {
            _playerName.text = playerName;
            
            _hpGauge.Setup(maxHp);
        }

        public void Dispose()
        {
            _sequence?.Kill();
            _scoreSeq?.Kill();
        }

        public void Reset()
        {
            _openFlag = false;
            SetActive(false);

            _hpGauge.SetHpImmediate(0);
        }

        public void SetActive(bool flag)
        {
            if (_group == null)
                return;

            _group.alpha = flag ? 1f : 0f;
            _group.blocksRaycasts = flag;
        }

        public void SetScore(int score)
        {
            _scoreSeq?.Kill();
            
            _score.text = score.ToString();
            _score.transform.localScale = Vector3.one * 1.5f;

            _scoreSeq = DOTween.Sequence()
                .Append(_score.transform.DOScale(1f, 0.2f).SetEase(Ease.OutQuad))
                .SetLink(gameObject)
                .Play();
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

            _openFlag = true;

            _sequence?.Kill();

            SetActive(false);

            _sequence = DOTween.Sequence()
                .Append(_group.DOFade(1f, 0.3f))
                .OnComplete(() => { _hpGauge.Play(() => SetActive(true)); })
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