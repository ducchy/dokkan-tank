using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace dtank
{
    public class BattleTankStatusUiView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Image[] _hpImages;

        private bool _openFlag;
        private int _hp;
        
        private Sequence _sequence;
        private Sequence _changeHpSequence;

        public void Setup()
        {
            _openFlag = false;
            SetActive(false);
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
            _changeHpSequence?.Kill();
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

            PlayChangeHp(_hp, hp);

            _hp = hp;
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
                .OnComplete(() =>
                {
                    foreach (var hpImage in _hpImages)
                        hpImage.enabled = true;

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
            if (!_openFlag || from == to)
                return;
            
            _changeHpSequence?.Complete();
            _changeHpSequence = DOTween.Sequence()
                .SetLink(gameObject);
            
            if (from < to) {
                for (var i = to ; i < from; i ++)
                {
                    var hpImage = _hpImages[i];
                    hpImage.color = new Color(1f, 1f, 1f, 0f);
                    hpImage.transform.localScale = Vector3.one * 0.5f;

                    _changeHpSequence
                        .Append(hpImage.DOFade(1f, 0.3f))
                        .Join(hpImage.transform.DOScale(1f, 0.3f));
                }
            }
            else
            {
                for (var i = from - 1 ; i >= to ; i--)
                {
                    var hpImage = _hpImages[i];
                    hpImage.color = new Color(1f, 1f, 1f, 1f);
                    hpImage.transform.localScale = Vector3.one;

                    _changeHpSequence
                        .Append(hpImage.DOFade(0f, 0.3f))
                        .Join(hpImage.transform.DOScale(0.5f, 0.3f));
                }
            }
            
            _changeHpSequence
                .OnComplete(() => onComplete?.Invoke())
                .Play();
        }
    }
}