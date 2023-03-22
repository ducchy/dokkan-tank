using System;
using GameFramework.Core;
using GameFramework.TaskSystems;
using UniRx;
using UnityEngine;

namespace dtank
{
    public class BattleCameraController : MonoBehaviour, ILateUpdatableTask, IDisposable
    {
        [SerializeField] private BattleCamera _camera;

        private BattleTankModel _playerTankModel;
        private TankActorContainer _tankActorContainer;
        private readonly DisposableScope _scope = new DisposableScope();

        public bool IsActive => isActiveAndEnabled;

        private readonly Subject<Unit> _onEndReadySubject = new Subject<Unit>();
        public IObservable<Unit> OnEndReadyAsObservable => _onEndReadySubject;

        public void Setup(
            BattleTankModel playerTankModel,
            TankActorContainer tankActorContainer)
        {
            _playerTankModel = playerTankModel;
            _tankActorContainer = tankActorContainer;
            
            SetEvent();
        }

        public void Dispose()
        {
            _onEndReadySubject.Dispose();
            _camera.Dispose();
        }

        void ITask.Update()
        {
        }

        void ILateUpdatableTask.LateUpdate()
        {
        }

        private void SetEvent()
        {
            _camera.OnEndReadyAsObservable
                .TakeUntil(_scope)
                .Subscribe(_ => _onEndReadySubject.OnNext(Unit.Default))
                .ScopeTo(_scope);
        }
        
        public void PlayReady()
        {
            var player = _tankActorContainer.ActorDictionary[_playerTankModel.OwnerId];
            _camera.SetFollowTarget(player.transform);
            _camera.PlayReady();
        }

        public void SkipReady()
        {
            _camera.SkipReady();
        }

        public void PlayResult(int winnerId)
        {
            var winner = _tankActorContainer.ActorDictionary[winnerId];
            _camera.SetFollowTarget(winner.transform);
            _camera.PlayResult();
        }

        public void EndResult()
        {
            _camera.EndResult();
        }
    }
}