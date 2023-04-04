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

        public int TopPlayerId { get; private set; }

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

            UpdateRanking();
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
            UpdateRanking();
            _resultType.Value = TopPlayerId == _mainPlayerId ? BattleResultType.Win : BattleResultType.Lose;
        }

        public void Dead(int id)
        {
            var remainTankCount = _tankModels.Count(model => !model.DeadFlag.Value);
            var deadTankModel = _tankModels.FirstOrDefault(model => model.Id == id);
            deadTankModel?.SetRank(remainTankCount + 1);

            UpdateRanking();
            if (id == _mainPlayerId)
            {
                _resultType.Value = BattleResultType.Lose;
                return;
            }

            if (remainTankCount <= 1)
                _resultType.Value = BattleResultType.Win;
        }

        public void UpdateRanking()
        {
            Debug.Log($"[BattleRuleModel] UpdateRanking");

            var ranking = _tankModels.Where(model => !model.DeadFlag.Value)
                .OrderByDescending(model => model.Score.Value)
                .ThenBy(model => model.Id)
                .ToList();

            for (var i = 0; i < ranking.Count(); i++)
                ranking[i].SetRank(i + 1);

            TopPlayerId = ranking[0].Id;
        }
    }
}