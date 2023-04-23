using System;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace dtank
{
    public class TitleUiView : MonoBehaviour, IDisposable
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private PlayableDirector _director;
        [SerializeField] private Button _startButton;

        [SerializeField] private PlayableAsset _enterPlayable;
        [SerializeField] private PlayableAsset _exitPlayable;

        public IObservable<Unit> OnStartButtonClickObservable => _startButton.OnClickAsObservable();

        private readonly Subject<Unit> _completeEnterSubject = new();
        public IObservable<Unit> CompleteEnterObservable => _completeEnterSubject;

        private readonly Subject<Unit> _completeExitSubject = new();
        public IObservable<Unit> CompleteExitObservable => _completeExitSubject;

        public void Setup()
        {
            _group.alpha = 0f;
            _group.blocksRaycasts = false;

            _director.stopped += OnStopped;
        }

        public void Dispose()
        {
            _director.stopped -= OnStopped;
        }

        public void PlayEnter()
        {
            _group.alpha = 1f;
            Play(_enterPlayable);
        }

        public void PlayExit()
        {
            Play(_exitPlayable);
        }

        private void Play(PlayableAsset playable)
        {
            if (_director.state == PlayState.Playing) 
                _director.Stop();

            _group.blocksRaycasts = false;

            _director.Play(playable);
        }

        private void OnStopped(PlayableDirector director)
        {
            if (director.playableAsset == _enterPlayable)
                _completeEnterSubject.OnNext(Unit.Default);
            else if(director.playableAsset == _exitPlayable)
                _completeExitSubject.OnNext(Unit.Default);
            
            _group.blocksRaycasts = true;
        }
    }
}