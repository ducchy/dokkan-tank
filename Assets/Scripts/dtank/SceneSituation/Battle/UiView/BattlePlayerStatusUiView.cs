using System;
using System.Collections.Generic;
using UnityEngine;

namespace dtank
{
    public class BattlePlayerStatusUiView : MonoBehaviour, IDisposable
    {
        [SerializeField] private BattleMainPlayerStatusUiView _mainPlayerStatus;
        [SerializeField] private BattleOtherPlayerStatusUiView _otherPlayerStatusPrefab;
        [SerializeField] private Transform _otherPlayerStatusParent;

        private readonly Dictionary<int, BattlePlayerStatusUiViewBase> _statusUiDictionary = new();

        public void Setup(BattleModel model)
        {
            _statusUiDictionary.Clear();
            foreach (var tankModel in model.TankModels)
            {
                BattlePlayerStatusUiViewBase statusUi = tankModel.CharacterType == CharacterType.Player
                    ? _mainPlayerStatus
                    : Instantiate(_otherPlayerStatusPrefab, _otherPlayerStatusParent);
                statusUi.Setup(tankModel.Name, tankModel.ParameterData.hp, tankModel.ActorModel.SetupData.color);
                _statusUiDictionary.Add(tankModel.Id, statusUi);
            }
        }

        public void Dispose()
        {
            _mainPlayerStatus.Dispose();
        }

        public void Reset()
        {
            foreach (var statusUi in _statusUiDictionary.Values)
                statusUi.Reset();
        }

        public void Open()
        {
            foreach (var statusUi in _statusUiDictionary.Values)
                statusUi.Open();
        }

        public void Close()
        {
            foreach (var statusUi in _statusUiDictionary.Values)
                statusUi.Close();
        }

        public BattlePlayerStatusUiViewBase GetStatusUi(int id)
        {
            return _statusUiDictionary.TryGetValue(id, out var value) ? value : null;
        }
    }
}