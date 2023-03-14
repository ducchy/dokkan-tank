using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace dtank
{
    public class BattleUiView : MonoBehaviour
    {
        [SerializeField] private Image _screenFill;

        private Sequence _sequence;

        public Action OnBeginBattleListener;
        public Action OnEndBattleListener;
        public Action OnEndPlayingListener;
        public Action OnBeginResultListener;

        public void Construct()
        {
            _screenFill.color = Color.black;
        }

        private void OnDestroy()
        {
            OnBeginBattleListener = null;
            OnEndBattleListener = null;
            OnEndPlayingListener = null;
            OnBeginResultListener = null;
        }

        private void SetActive(bool active)
        {
            _screenFill.enabled = active;
        }

        public void BeginBattle()
        {
            PlayFadeIn(() => OnBeginBattleListener?.Invoke());
        }

        public void EndBattle()
        {
            PlayFadeOut(() => OnEndBattleListener?.Invoke());
        }

        public void EndPlaying()
        {
            PlayFadeOut(() => OnEndPlayingListener?.Invoke());
        }

        public void BeginResult()
        {
            PlayFadeIn(() => OnBeginResultListener?.Invoke());
        }

        private void PlayFadeIn(Action onComplete = null)
        {
            _sequence?.Kill();
            
            _screenFill.color = Color.black;

            _sequence = DOTween.Sequence()
                .AppendInterval(0.3f)
                .Append(_screenFill.DOFade(0f, 0.3f))
                .OnComplete(() => onComplete?.Invoke())
                .SetLink(gameObject)
                .Play();
        }

        private void PlayFadeOut(Action onComplete = null)
        {
            _sequence?.Kill();
            
            _screenFill.color = Color.clear;

            _sequence = DOTween.Sequence()
                .Append(_screenFill.DOFade(1f, 0.3f))
                .AppendInterval(0.3f)
                .OnComplete(() => onComplete?.Invoke())
                .SetLink(gameObject)
                .Play();
        }
    }
}