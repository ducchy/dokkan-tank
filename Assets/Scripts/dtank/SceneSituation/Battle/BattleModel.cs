using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Core;
using GameFramework.CoroutineSystems;
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

        private List<BattleTankModel> _tankModels;
        public IReadOnlyList<BattleTankModel> TankModels => _tankModels;
        
        private readonly Dictionary<int, IBehaviourSelector> _behaviourSelectorDictionary = new Dictionary<int, IBehaviourSelector>();

        public BattleRuleModel RuleModel { get; private set; }
        public IReadOnlyDictionary<int, IBehaviourSelector> BehaviourSelectorDictionary => _behaviourSelectorDictionary;

        public event Action<BattleModel> OnUpdated;
        private readonly CoroutineRunner _coroutineRunner = new CoroutineRunner();

        bool ITask.IsActive => true;

        private BattleModel(object empty) : base(empty)
        {
        }
        
        public IObservable<BattleModel> OnUpdatedAsObservable() {
            return Observable.FromEvent<BattleModel>(
                h => OnUpdated += h,
                h => OnUpdated -= h);
        }

        public IObservable<Unit> SetupAsync(BattleEntryData entryData, FieldViewData fieldViewData)
        {
            return Observable.Defer(() =>
                {
                    var scope = new DisposableScope();
                    return _coroutineRunner.StartCoroutineAsync(SetupRoutine(entryData, fieldViewData, scope),
                        () => { scope.Dispose(); });
                })
                .Do(_ => OnUpdated?.Invoke(this));
        }

        public IEnumerator SetupRoutine(BattleEntryData entryData, FieldViewData fieldViewData, IScope scope)
        {
            var ruleData = default(BattleRuleData);
            yield return new BattleRuleDataAssetRequest($"{entryData.RuleId:d3}")
                .LoadAsync(scope)
                .Do(x => ruleData = x)
                .StartAsEnumerator(scope);
            RuleModel = new BattleRuleModel(ruleData.duration);
            
            _tankModels = new List<BattleTankModel>();
            foreach (var player in entryData.Players)
            {
                var startPointData = fieldViewData.StartPointDataArray[player.PositionIndex];
                
                var parameterData = default(TankParameterData);
                yield return new BattleTankParameterDataAssetRequest($"{player.ParameterId:d3}")
                    .LoadAsync(scope)
                    .Do(x => parameterData = x)
                    .StartAsEnumerator(scope);

                var tankModel = BattleTankModel.Create();
                tankModel.Setup(player.Name, player.ModelId, player.CharacterType, startPointData, parameterData);
                _tankModels.Add(tankModel);
            }

            _behaviourSelectorDictionary.Clear();
            foreach (var tankModel in _tankModels)
            {
                if (tankModel.CharacterType == CharacterType.Player)
                    continue;

                var npcBehaviourSelector =
                    new NpcBehaviourSelector(tankModel, _tankModels.Where(m => m != tankModel).ToArray());
                _behaviourSelectorDictionary.Add(tankModel.Id, npcBehaviourSelector);
            }
                
            Bind();
        }

        protected override void OnDeletedInternal()
        {
            base.OnDeletedInternal();

            RuleModel.Dispose();
            foreach (var tankModel in _tankModels)
                BattleTankModel.Delete(tankModel.Id);
            foreach (var pair in _behaviourSelectorDictionary)
                pair.Value.Dispose();

            _tankModels.Clear();
            _behaviourSelectorDictionary.Clear();
            _coroutineRunner.Dispose();
            _currentState.Dispose();
        }

        private void Bind()
        {
            foreach (var tankModel in _tankModels)
            {
                tankModel.BattleState
                    .Subscribe(state =>
                    {
                        if (state == BattleTankState.Dead)
                            RuleModel.Dead(tankModel.Id);
                    })
                    .ScopeTo(this);
            }
        }

        void ITask.Update()
        {
            _coroutineRunner.Update();
            RuleModel?.Update();
            foreach (var tankModel in _tankModels)
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