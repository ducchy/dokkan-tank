using System;
using GameFramework.TaskSystems;
using UnityEngine;

namespace dtank
{
    public class BattleCameraController : MonoBehaviour, ILateUpdatableTask, IDisposable
    {
        [SerializeField] private BattleCamera _camera;

        private BattleTankModel _playerTankModel;
        private TankActorContainer _tankActorContainer;

        public bool IsActive => isActiveAndEnabled;

        public Action OnEndReadyAction;

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
            OnEndReadyAction = null;
        }

        void ITask.Update()
        {
        }

        void ILateUpdatableTask.LateUpdate()
        {
        }

        private void SetEvent()
        {
            _camera.OnEndReadyAction = OnEndReady;
        }
        
        public void PlayReady()
        {
            var player = _tankActorContainer.ActorDictionary[_playerTankModel.Data.OwnerId];
            _camera.SetFollowTarget(player.transform);
            _camera.PlayReady();
        }

        public void SkipReady()
        {
            _camera.SkipReady();
        }

        private void OnEndReady()
        {
            OnEndReadyAction?.Invoke();
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