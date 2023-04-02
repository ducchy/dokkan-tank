using System;
using DG.Tweening;
using GameFramework.Core;
using GameFramework.TaskSystems;
using UniRx;
using UnityEngine;

namespace dtank
{
    public class BattleUiView : MonoBehaviour, IDisposable, ITask
    {
        [SerializeField] private BattleTankControlUiView _tankControlUiView;
        [SerializeField] private BattlePlayerStatusUiView _playerStatusUiView;
        [SerializeField] private BattleReadyUiView _readyUiView;
        [SerializeField] private BattlePlayingUiView _playingUiView;
        [SerializeField] private BattleResultUiView _resultUiView;

        public BattleTankControlUiView TankControlUiView => _tankControlUiView;
        public BattlePlayerStatusUiView PlayerStatusUiView => _playerStatusUiView;
        public BattleReadyUiView ReadyUiView => _readyUiView;
        public BattlePlayingUiView PlayingUiView => _playingUiView;
        public BattleResultUiView ResultUiView => _resultUiView;

        bool ITask.IsActive => isActiveAndEnabled;

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

            _tankControlUiView.Setup();
        }

        public void Dispose()
        {
            _tankControlUiView.Dispose();
            _playerStatusUiView.Dispose();
            _readyUiView.Dispose();
            _playingUiView.Dispose();
            _resultUiView.Dispose();

            _sequence.Kill();
            _onEndPlayingSubject.Dispose();
            _onBeginResultSubject.Dispose();
        }

        public void Reset()
        {
            _tankControlUiView.Reset();
            _playerStatusUiView.Reset();
            _readyUiView.Reset();
            _playingUiView.Reset();
            _resultUiView.Reset();
        }

        void ITask.Update()
        {
        }

        public void EndPlaying()
        {
            _fadeScope.Dispose();
            _fadeController.FadeOutAsync(Color.black, 0.5f)
                .Delay(TimeSpan.FromSeconds(1f))
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