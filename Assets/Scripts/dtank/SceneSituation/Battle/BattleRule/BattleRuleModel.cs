using System.Collections.Generic;
using GameFramework.ModelSystems;
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

    public class BattleRuleModel : SingleModel<BattleRuleModel>
    {
        private readonly ReactiveProperty<BattleResultType> _resultType =
            new ReactiveProperty<BattleResultType>(BattleResultType.None);

        public IReadOnlyReactiveProperty<BattleResultType> ResultType => _resultType;

        private readonly ReactiveProperty<int> _remainTimeInt = new ReactiveProperty<int>();
        public IReadOnlyReactiveProperty<int> RemainTime => _remainTimeInt;

        private float _remainTime;
        private bool _playingFlag;

        private readonly Dictionary<int, int> _scoreDictionary = new Dictionary<int, int>();
        private float _duration;

        public int WinnerId { get; private set; }

        public void Setup(float duration)
        {
            _duration = 5f;
        }

        public void SetResultType(BattleResultType resultType)
        {
            _resultType.Value = resultType;
        }

        public void Update(float deltaTime)
        {
            if (!_playingFlag)
                return;

            _remainTime -= deltaTime;
            _remainTimeInt.Value = Mathf.CeilToInt(_remainTime);

            if (_remainTime < 0f)
                TimeUp();
        }

        public void Ready()
        {
            _playingFlag = false;

            _resultType.Value = BattleResultType.None;

            _remainTime = _duration;
            _remainTimeInt.Value = Mathf.CeilToInt(_remainTime);

            _scoreDictionary.Clear();
            _scoreDictionary.Add(1, 0);
            _scoreDictionary.Add(2, 0);
            _scoreDictionary.Add(3, 0);
            _scoreDictionary.Add(4, 0);
        }

        public void Start()
        {
            _playingFlag = true;
        }

        private void TimeUp()
        {
            _playingFlag = false;
            WinnerId = GetTopPlayerId();
            _resultType.Value = WinnerId == 1 ?  BattleResultType.Win : BattleResultType.Lose;
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
            if (id == 1)
                _resultType.Value = BattleResultType.Lose;
            else if (_scoreDictionary.Count <= 1)
                _resultType.Value = BattleResultType.Win;
        }

        private int GetTopPlayerId()
        {
            int topPlayerId = 1;
            int maxScore = int.MinValue;
            foreach (var pair in _scoreDictionary)
            {
                if (maxScore >= pair.Value)
                    continue;

                topPlayerId = pair.Key;
                maxScore = pair.Value;
            }

            return topPlayerId;
        }

        private BattleRuleModel(object empty) : base(empty)
        {
        }
    }
}