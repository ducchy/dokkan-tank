using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

namespace dtank
{
    public class HpGaugeView : MonoBehaviour, IDisposable
    {
        [SerializeField] private Color _hpEnableColor;
        [SerializeField] private Color _hpDisableColor;

        [SerializeField] private Image[] _hpImages;

        public int DisplayHp { get; private set; }
        public int CurrentHp { get; private set; }

        private Sequence _sequence;

        public void Dispose()
        {
            _sequence?.Kill();
        }

        public void SetHp(int hp)
        {
            if (CurrentHp == hp)
                return;

            Debug.Log($"[BattleTankStatusUiView] SetHp: {CurrentHp} -> {hp}");

            CurrentHp = hp;
        }

        public void SetHpImmediate(int hp)
        {
            for (var i = 0; i < _hpImages.Length; i++)
                _hpImages[i].color = i < hp ? _hpEnableColor : _hpDisableColor;

            CurrentHp = DisplayHp = hp;
        }

        public void Play(Action onComplete = null)
        {
            PlayChangeHp(DisplayHp, CurrentHp, onComplete);

            DisplayHp = CurrentHp;
        }

        private void PlayChangeHp(int from, int to, Action onComplete = null)
        {
            if (from == to)
            {
                onComplete?.Invoke();
                return;
            }

            Debug.Log($"[BattleTankStatusUiView] PlayChangeHp: {from} -> {to}");

            _sequence?.Complete();
            _sequence = DOTween.Sequence()
                .SetLink(gameObject);

            if (from < to)
            {
                for (var i = from; i < to; i++)
                {
                    var hpImage = _hpImages[i];
                    hpImage.color = _hpDisableColor;
                    hpImage.transform.localScale = Vector3.one * 0.5f;

                    _sequence
                        .Append(hpImage.DOColor(_hpEnableColor, 0.3f))
                        .Join(hpImage.transform.DOScale(1f, 0.3f));
                }
            }
            else
            {
                for (var i = from - 1; i >= to; i--)
                {
                    var hpImage = _hpImages[i];
                    hpImage.color = _hpEnableColor;
                    hpImage.transform.localScale = Vector3.one;

                    _sequence
                        .Append(hpImage.DOColor(_hpDisableColor, 0.3f))
                        .Join(hpImage.transform.DOScale(0.5f, 0.3f));
                }
            }

            _sequence
                .OnComplete(() => onComplete?.Invoke())
                .Play();
        }
    }
}