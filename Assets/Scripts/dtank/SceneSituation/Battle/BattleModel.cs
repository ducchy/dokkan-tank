using System.Collections.Generic;
using GameFramework.Core;
using GameFramework.ModelSystems;
using GameFramework.TaskSystems;
using UniRx;
using UnityEngine;

namespace dtank
{
    public class BattleModel : SingleModel<BattleModel>, ITask
    {
        private readonly ReactiveProperty<BattleState> _currentState = new ReactiveProperty<BattleState>();
        public IReadOnlyReactiveProperty<BattleState> CurrentState => _currentState;

        private Dictionary<int, BattleTankModel> _tankModelDictionary;
        private Dictionary<int, IBehaviourSelector> _behaviourSelectorDictionary;

        public BattleRuleModel RuleModel { get; private set; }
        public IReadOnlyDictionary<int, BattleTankModel> TankModelDictionary => _tankModelDictionary;
        public IReadOnlyDictionary<int, IBehaviourSelector> BehaviourSelectorDictionary => _behaviourSelectorDictionary;

        public bool IsActive { get; private set; }

        private BattleModel(object empty) : base(empty)
        {
        }

        public void Setup(
            BattleRuleModel ruleModel,
            Dictionary<int, BattleTankModel> tankModelDictionary,
            Dictionary<int, IBehaviourSelector> behaviourSelectorDictionary
        )
        {
            RuleModel = ruleModel;
            _tankModelDictionary = tankModelDictionary;
            _behaviourSelectorDictionary = behaviourSelectorDictionary;

            Bind();
        }

        protected override void OnDeletedInternal()
        {
            base.OnDeletedInternal();

            RuleModel.Dispose();
            foreach (var pair in _tankModelDictionary)
                pair.Value.Dispose();
            foreach (var pair in _behaviourSelectorDictionary)
                pair.Value.Dispose();

            _tankModelDictionary.Clear();
            _behaviourSelectorDictionary.Clear();
        }

        private void Bind()
        {
            foreach (var tankModel in _tankModelDictionary.Values)
            {
                tankModel.BattleState
                    .Subscribe(state =>
                    {
                        if (state == BattleTankState.Dead)
                            RuleModel.Dead(tankModel.Data.OwnerId);
                    })
                    .ScopeTo(this);
            }
        }

        public void Update()
        {
            RuleModel.Update();
            foreach (var tankModel in _tankModelDictionary.Values)
                tankModel.Update();
            foreach (var behaviourSelector in _behaviourSelectorDictionary.Values)
                if (behaviourSelector is NpcBehaviourSelector npcBehaviourSelector)
                    npcBehaviourSelector.Update();
        }

        public void ChangeState(BattleState state)
        {
            Debug.LogFormat("BattleModel.ChangeState(): state={0}", state);

            if (state == BattleState.Retry)
                state = BattleState.Ready;

            _currentState.Value = state;

            switch (state)
            {
                case BattleState.Ready:
                    RuleModel.Ready();
                    break;
                case BattleState.Playing:
                    RuleModel.Start();
                    break;
            }

            IsActive = IsActiveState(state);
        }

        private bool IsActiveState(BattleState state)
        {
            switch (state)
            {
                case BattleState.Invalid:
                case BattleState.Retry:
                case BattleState.Quit:
                    return false;
                default:
                    return true;
            }
        }
    }
}