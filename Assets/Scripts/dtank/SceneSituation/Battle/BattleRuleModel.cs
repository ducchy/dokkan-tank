using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace dtank
{
    public enum BattleResultType
    {
        None,
        Win,
        Lose,
    }

    public class BattleRuleModel : IDisposable
    {
        private readonly ReactiveProperty<BattleResultType> _resultType =
            new ReactiveProperty<BattleResultType>(BattleResultType.None);

        public IReadOnlyReactiveProperty<BattleResultType> ResultType => _resultType;

        private readonly ReactiveProperty<int> _remainTimeInt = new ReactiveProperty<int>();
        public IReadOnlyReactiveProperty<int> RemainTime => _remainTimeInt;

        private float _remainTime;
        private bool _playingFlag;

        private readonly Dictionary<int, int> _scoreDictionary = new Dictionary<int, int>();
        private readonly float _duration;
        private readonly int _mainPlayerId;
        private readonly int[] _playerIds;
        private bool _isActive;

        public int WinnerId { get; private set; }

        public BattleRuleModel(float duration, int mainPlayerId, int[] playerIds)
        {
            _duration = duration;
            _mainPlayerId = mainPlayerId;
            _playerIds = playerIds;
        }

        public void Dispose()
        {
            _scoreDictionary.Clear();
        }

        public void Update()
        {
            if (!_playingFlag)
                return;

            _remainTime -= Time.deltaTime;
            _remainTimeInt.Value = Mathf.CeilToInt(_remainTime);

            if (_remainTime < 0f)
                TimeUp();
        }

        public void Reset()
        {
            _playingFlag = false;

            _resultType.Value = BattleResultType.None;

            _remainTime = _duration;
            _remainTimeInt.Value = Mathf.CeilToInt(_remainTime);

            _scoreDictionary.Clear();
            foreach (var playerId in _playerIds)
                _scoreDictionary.Add(playerId, 0);
        }

        public void Start()
        {
            _playingFlag = true;
        }

        public void ForceEnd()
        {
            TimeUp();
        }

        private void TimeUp()
        {
            _playingFlag = false;
            WinnerId = GetTopPlayerId();
            _resultType.Value = WinnerId == _mainPlayerId ? BattleResultType.Win : BattleResultType.Lose;
        }

        public void IncrementScore(int playerId)
        {
            if (!_scoreDictionary.ContainsKey(playerId))
                return;

            _scoreDictionary[playerId]++;
            WinnerId = GetTopPlayerId();
        }

        public void Dead(int id)
        {
            _scoreDictionary.Remove(id);
            WinnerId = GetTopPlayerId();
            if (id == _mainPlayerId)
                _resultType.Value = BattleResultType.Lose;
            else if (_scoreDictionary.Count <= 1)
                _resultType.Value = BattleResultType.Win;
        }

        private int GetTopPlayerId()
        {
            var topPlayerId = _mainPlayerId;
            var maxScore = int.MinValue;
            foreach (var pair in _scoreDictionary)
            {
                if (maxScore >= pair.Value)
                    continue;

                topPlayerId = pair.Key;
                maxScore = pair.Value;
            }

            return topPlayerId;
        }
    }
}