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

        public Action OnBeginBattleAction;
        public Action OnQuitBattleAction;
        public Action OnRetryBattleAction;
        public Action OnEndPlayingAction;
        public Action OnBeginResultAction;

        public void Setup()
        {
            _screenFill.color = Color.black;
        }

        private void OnDestroy()
        {
            OnBeginBattleAction = null;
            OnQuitBattleAction = null;
            OnRetryBattleAction = null;
            OnEndPlayingAction = null;
            OnBeginResultAction = null;
        }

        private void SetActive(bool active)
        {
            _screenFill.enabled = active;
        }

        public void BeginBattle()
        {
            PlayFadeIn(() => OnBeginBattleAction?.Invoke());
        }

        public void QuitBattle()
        {
            PlayFadeOut(() => OnQuitBattleAction?.Invoke());
        }

        public void RetryBattle()
        {
            PlayFadeOut(() => OnRetryBattleAction?.Invoke());
        }

        public void EndPlaying()
        {
            PlayFadeOut(() => OnEndPlayingAction?.Invoke());
        }

        public void BeginResult()
        {
            PlayFadeIn(() => OnBeginResultAction?.Invoke());
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