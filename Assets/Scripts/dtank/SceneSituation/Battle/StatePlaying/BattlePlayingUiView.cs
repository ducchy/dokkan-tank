using System;
using DG.Tweening;
using GameFramework.Core;
using TMPro;
using UniRx;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace dtank
{
    public class BattlePlayingUiView : MonoBehaviour, IDisposable
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private TextMeshProUGUI _centerLabel;
        [SerializeField] private TextMeshProUGUI _remainTime;

        private Sequence _seq;
        private Sequence _finishSeq;
        private FadeController _fadeController;
        private readonly DisposableScope _fadeScope = new();

        private readonly Subject<Unit> _onEndPlayingSubject = new();
        public IObservable<Unit> OnEndPlayingAsObservable => _onEndPlayingSubject;

        public void Setup(FadeController fadeController)
        {
            _fadeController = fadeController;
        }

        public void Dispose()
        {
            _seq?.Kill();
            _finishSeq?.Kill();

            _fadeScope.Dispose();
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

        public void Start()
        {
            _seq?.Kill();

            SetActive(true);

            _centerLabel.text = "START!";
            _centerLabel.color = new Color(0.2f, 0.2f, 0.2f, 0f);
            _centerLabel.transform.localScale = Vector3.one * 3f;

            _seq = DOTween.Sequence()
                .Append(_centerLabel.DOFade(1f, 0.3f).SetEase(Ease.OutQuad))
                .Join(_centerLabel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutQuad))
                .AppendInterval(1f)
                .Append(_centerLabel.DOFade(0f, 0.3f))
                .Join(_centerLabel.transform.DOScale(0.5f, 0.3f))
                .SetLink(gameObject)
                .Play();
        }

        public void Finish()
        {
            _seq?.Kill();
            _fadeScope.Dispose();

            _centerLabel.text = "FINISH!";
            _centerLabel.color = new Color(0.2f, 0.2f, 0.2f, 0f);
            _centerLabel.transform.localScale = Vector3.one * 0.5f;

            _seq = DOTween.Sequence()
                .Append(_centerLabel.DOFade(1f, 0.3f))
                .Join(_centerLabel.transform.DOScale(1f, 0.3f))
                .AppendInterval(1f)
                .Append(_centerLabel.DOFade(0f, 0.3f))
                .Join(_centerLabel.transform.DOScale(0.5f, 0.3f))
                .OnComplete(() =>
                {
                    SetActive(false);
                    
                    _fadeController.FadeOut(Color.black, 0.5f,
                        () => _onEndPlayingSubject.OnNext(Unit.Default), _fadeScope);
                })
                .SetLink(gameObject)
                .Play();
        }

        public void SetTime(int second)
        {
            _remainTime.text = second.ToString();
        }
    }
}