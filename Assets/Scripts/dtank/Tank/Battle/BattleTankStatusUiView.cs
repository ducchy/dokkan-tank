using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

namespace dtank
{
    public class BattleTankStatusUiView : MonoBehaviour, IDisposable
    {
        [SerializeField] private Color _hpEnableColor;
        [SerializeField] private Color _hpDisableColor;
        
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Image[] _hpImages;

        private bool _openFlag;
        private int _hp;
        
        private Sequence _sequence;
        private Sequence _changeHpSequence;

        public void Dispose()
        {
            _sequence?.Kill();
            _changeHpSequence?.Kill();
        }

        public void Reset()
        {
            _openFlag = false;
            SetActive(false);
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
            if (_hp == hp)
                return;
            
            Debug.Log($"[BattleTankStatusUiView] SetHp: hp={hp}");
            
            PlayChangeHp(_hp, hp);

            _hp = hp;
        }

        public void Open()
        {
            if (_openFlag)
                return;
            
            Debug.Log("[BattleTankStatusUiView] Open");

            _openFlag = true;
            
            _sequence?.Kill();
            
            SetActive(false);
            
            foreach (var hpImage in _hpImages)
                hpImage.color = _hpDisableColor;

            _sequence = DOTween.Sequence()
                .Append(_group.DOFade(1f, 0.3f))
                .OnComplete(() =>
                {
                    PlayChangeHp(0, _hp, () => SetActive(true));
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

        private void PlayChangeHp(int from, int to, Action onComplete = null)
        {
            Debug.Log($"[BattleTankStatusUiView] PlayChangeHp: {from} -> {to}");

            if (!_openFlag || from == to)
                return;
            
            _changeHpSequence?.Complete();
            _changeHpSequence = DOTween.Sequence()
                .SetLink(gameObject);
            
            if (from < to) {
                for (var i = from ; i < to; i ++)
                {
                    var hpImage = _hpImages[i];
                    hpImage.color = _hpDisableColor;
                    hpImage.transform.localScale = Vector3.one * 0.5f;

                    _changeHpSequence
                        .Append(hpImage.DOColor(_hpEnableColor, 0.3f))
                        .Join(hpImage.transform.DOScale(1f, 0.3f));
                }
            }
            else
            {
                for (var i = from - 1 ; i >= to ; i--)
                {
                    var hpImage = _hpImages[i];
                    hpImage.color = _hpEnableColor;
                    hpImage.transform.localScale = Vector3.one;

                    _changeHpSequence
                        .Append(hpImage.DOColor(_hpDisableColor, 0.3f))
                        .Join(hpImage.transform.DOScale(0.5f, 0.3f));
                }
            }
            
            _changeHpSequence
                .OnComplete(() => onComplete?.Invoke())
                .Play();
        }
    }
}