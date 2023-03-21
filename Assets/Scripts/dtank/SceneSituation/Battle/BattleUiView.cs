using System;
using DG.Tweening;
using GameFramework.Core;
using GameFramework.CoroutineSystems;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace dtank
{
    public class BattleUiView : MonoBehaviour
    {
        private FadeController _fadeController;
        private Sequence _sequence;
        private readonly DisposableScope _fadeScope = new DisposableScope();

        private readonly Subject<Unit> _onEndPlayingSubject = new Subject<Unit>();
        public IObservable<Unit> OnEndPlayingAsObservable => _onEndPlayingSubject;

        private readonly Subject<Unit> _onBeginResultSubject = new Subject<Unit>();
        public IObservable<Unit> OnBeginResultAsObservable => _onBeginResultSubject;

        public void Setup(FadeController fadeController)
        {
            _fadeController = fadeController;
        }

        private void OnDestroy()
        {
            _onEndPlayingSubject.Dispose();
            _onBeginResultSubject.Dispose();
        }

        private void SetActive(bool active)
        {
        }

        public void EndPlaying()
        {
            _fadeScope.Dispose();
            _fadeController.FadeOutAsync(Color.black, 0.5f)
                .Subscribe(
                    onNext: _ => { },
                    onCompleted: () => _onEndPlayingSubject.OnNext(Unit.Default)
                )
                .ScopeTo(_fadeScope);
        }

        public void BeginResult()
        {
            _fadeScope.Dispose();
            _fadeController.FadeInAsync(0.5f)
                .Subscribe(
                    onNext: _ => { },
                    onCompleted: () => _onBeginResultSubject.OnNext(Unit.Default)
                )
                .ScopeTo(_fadeScope);
        }
    }
}