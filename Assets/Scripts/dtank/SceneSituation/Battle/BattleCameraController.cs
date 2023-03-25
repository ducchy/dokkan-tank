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
        private BattleTankActorContainer _battleTankActorContainer;
        private readonly DisposableScope _scope = new DisposableScope();

        public bool IsActive => isActiveAndEnabled;

        private readonly Subject<Unit> _onEndReadySubject = new Subject<Unit>();
        public IObservable<Unit> OnEndReadyAsObservable => _onEndReadySubject;

        public void Setup(
            BattleTankModel playerTankModel,
            BattleTankActorContainer battleTankActorContainer)
        {
            _playerTankModel = playerTankModel;
            _battleTankActorContainer = battleTankActorContainer;
            
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
            var player = _battleTankActorContainer.ActorDictionary[_playerTankModel.Id];
            _camera.SetFollowTarget(player.transform);
            _camera.PlayReady();
        }

        public void SkipReady()
        {
            _camera.SkipReady();
        }

        public void PlayResult(int winnerId)
        {
            var winner = _battleTankActorContainer.ActorDictionary[winnerId];
            _camera.SetFollowTarget(winner.transform);
            _camera.PlayResult();
        }

        public void EndResult()
        {
            _camera.EndResult();
        }
    }
}