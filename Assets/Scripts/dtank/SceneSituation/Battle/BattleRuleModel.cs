using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ReactiveProperty<BattleResultType> _resultType = new(BattleResultType.None);

        public IReadOnlyReactiveProperty<BattleResultType> ResultType => _resultType;

        private readonly ReactiveProperty<int> _remainTimeInt = new();
        public IReadOnlyReactiveProperty<int> RemainTime => _remainTimeInt;

        private float _remainTime;
        private bool _playingFlag;
        private bool _isActive;

        private readonly float _duration;
        private readonly int _mainPlayerId;
        private readonly IReadOnlyList<BattleTankModel> _tankModels;

        public int WinnerId { get; private set; }

        public BattleRuleModel(float duration, int mainPlayerId, IReadOnlyList<BattleTankModel> tankModels)
        {
            _duration = duration;
            _mainPlayerId = mainPlayerId;
            _tankModels = tankModels;
        }

        public void Dispose()
        {
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

        public void Dead(int id)
        {
            WinnerId = GetTopPlayerId();
            if (id == _mainPlayerId)
            {
                _resultType.Value = BattleResultType.Lose;
                return;
            }

            var remainPlayerCount = _tankModels.Count(model => !model.DeadFlag);
            if (remainPlayerCount <= 1)
                _resultType.Value = BattleResultType.Win;
        }

        private int GetTopPlayerId()
        {
            var topPlayerId = _mainPlayerId;
            var maxScore = int.MinValue;
            foreach (var tankModel in _tankModels)
            {
                if (tankModel.DeadFlag)
                    continue;

                if (maxScore >= tankModel.Score.Value)
                    continue;

                topPlayerId = tankModel.Id;
                maxScore = tankModel.Score.Value;
            }

            return topPlayerId;
        }
    }
}