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

        private bool _isActive;

        public readonly TimerModel TimerModel;
        private readonly int _mainPlayerId;
        private readonly IReadOnlyList<BattleTankModel> _tankModels;

        public int TopPlayerId { get; private set; }

        public BattleRuleModel(float duration, int mainPlayerId, IReadOnlyList<BattleTankModel> tankModels)
        {
            TimerModel = new TimerModel(duration);
            _mainPlayerId = mainPlayerId;
            _tankModels = tankModels;

            Bind();
        }

        public void Dispose()
        {
            _resultType.Dispose();
            TimerModel.Dispose();
        }

        private void Bind()
        {
            TimerModel.TimeUpObservable
                .Subscribe(_ => TimeUp());
        }

        public void Update(float deltaTime)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (DebugManager.BattleDebugModel.TimerStopFlag)
                return;
#endif
            TimerModel.Update(deltaTime);
        }

        public void Reset()
        {
            _resultType.Value = BattleResultType.None;

            TimerModel.SetActive(false);
            TimerModel.Reset();

            UpdateRanking();
        }

        public void Start()
        {
            Debug.Log($"[BattleRuleModel] Start");

            TimerModel.SetActive(true);
        }

        private void TimeUp()
        {
            if (_resultType.Value != BattleResultType.None)
                return;

            Debug.Log($"[BattleRuleModel] TimeUp");

            UpdateRanking();
            _resultType.Value = TopPlayerId == _mainPlayerId ? BattleResultType.Win : BattleResultType.Lose;
        }

        public void Dead(int id)
        {
            if (_resultType.Value != BattleResultType.None)
                return;

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

            var count = ranking.Count();

            if (count <= 0)
                return;

            for (var i = 0; i < count; i++)
                ranking[i].SetRank(i + 1);

            TopPlayerId = ranking[0].Id;
        }
    }
}