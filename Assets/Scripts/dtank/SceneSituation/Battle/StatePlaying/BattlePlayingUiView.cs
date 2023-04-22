using System;
using BrunoMikoski.AnimationSequencer;
using GameFramework.Core;
using TMPro;
using UniRx;
using UnityEngine;

namespace dtank
{
    public class BattlePlayingUiView : MonoBehaviour, IDisposable
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private TextMeshProUGUI _remainTime;
        [SerializeField] private AnimationSequencerController _startSeq;
        [SerializeField] private AnimationSequencerController _finishSeq;

        private FadeController _fadeController;
        private readonly DisposableScope _fadeScope = new();

        private readonly Subject<Unit> _onEndPlayingSubject = new();
        public IObservable<Unit> OnEndPlayingObservable => _onEndPlayingSubject;

        public void Setup(FadeController fadeController)
        {
            _fadeController = fadeController;
        }

        public void Dispose()
        {
            _startSeq.Kill();
            _finishSeq.Kill();

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
            _startSeq.Kill();

            SetActive(true);

            _startSeq.Play();
        }

        public void Finish()
        {
            _startSeq.Kill();
            _finishSeq.Kill();
            
            _fadeScope.Dispose();
            
            _finishSeq.Play(() =>
            {
                SetActive(false);
                    
                _fadeController.FadeOut(Color.black, 0.5f,
                    () => _onEndPlayingSubject.OnNext(Unit.Default), _fadeScope);
            });
        }

        public void SetTime(int second)
        {
            _remainTime.text = second.ToString();
        }
    }
}