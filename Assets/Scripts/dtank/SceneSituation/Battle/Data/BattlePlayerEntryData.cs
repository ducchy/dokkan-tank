using System;
using UnityEngine;

namespace dtank
{
    [Serializable]
    public class BattlePlayerEntryData
    {
        public int PlayerId => _playerId;
        public string Name => _name;
        public int BodyId => _bodyId;
        public int PositionIndex => _positionIndex;
        public CharacterType CharacterType => _characterType;
        public int ParameterId => _parameterId;
        
        [SerializeField] private int _playerId;
        [SerializeField] private string _name;
        [SerializeField] private int _bodyId;
        [SerializeField] private int _positionIndex;
        [SerializeField] private CharacterType _characterType;
        [SerializeField] private int _parameterId;

        public BattlePlayerEntryData(int playerId, string name, int bodyId, int positionIndex, CharacterType characterType, int parameterId)
        {
            _playerId = playerId;
            _name = name;
            _bodyId = bodyId;
            _positionIndex = positionIndex;
            _characterType = characterType;
            _parameterId = parameterId;
        }
    }
}