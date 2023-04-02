using System;
using UnityEngine;

namespace dtank
{
    public class BattlePlayerStatusUiView : MonoBehaviour, IDisposable
    {
        [SerializeField] private BattleMainPlayerStatusUiView _mainPlayerStatus;

        public BattleMainPlayerStatusUiView MainPlayerStatus => _mainPlayerStatus;

        public void Setup(BattleModel model)
        {
            foreach (var tankModel in model.TankModels)
            {
                if (tankModel.CharacterType == CharacterType.Player)
                    _mainPlayerStatus.Setup(tankModel);
            }
        }
        
        public void Dispose()
        {
            _mainPlayerStatus.Dispose();
        }

        public void Reset()
        {
            _mainPlayerStatus.Reset();
        }

        public void Open()
        {
            _mainPlayerStatus.Open();
        }

        public void Close()
        {
            _mainPlayerStatus.Close();
        }
    }
}