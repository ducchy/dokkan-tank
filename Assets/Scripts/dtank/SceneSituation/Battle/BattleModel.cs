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

        private readonly List<BattleTankModel> _tankModels = new List<BattleTankModel>();
        public IReadOnlyList<BattleTankModel> TankModels => _tankModels;
        
        public BattleTankModel MainPlayerTankModel { get; private set; }

        public BattleRuleModel RuleModel { get; private set; }

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
            
            _tankModels.Clear();
            foreach (var player in entryData.Players)
            {
                var parameterData = default(BattleTankParameterData);
                yield return new BattleTankParameterDataAssetRequest($"{player.ParameterId:d3}")
                    .LoadAsync(scope)
                    .Do(x => parameterData = x)
                    .StartAsEnumerator(scope);

                var tankModel = BattleTankModel.Create();
                tankModel.Update(player.Name, player.BodyId, player.CharacterType, parameterData);
                
                var actorSetupData = default(BattleTankActorSetupData);
                yield return new BattleTankActorSetupDataAssetRequest($"{player.ParameterId:d3}")
                    .LoadAsync(scope)
                    .Do(x => actorSetupData = x)
                    .StartAsEnumerator(scope);
                
                var startPointData = fieldViewData.StartPointDataArray[player.PositionIndex];
                
                tankModel.ActorModel.Update(actorSetupData, startPointData);
                
                _tankModels.Add(tankModel);

                if (MainPlayerTankModel == null && tankModel.CharacterType == CharacterType.Player)
                    MainPlayerTankModel = tankModel;
            }
            
            Bind();
        }

        protected override void OnDeletedInternal()
        {
            base.OnDeletedInternal();

            RuleModel.Dispose();
            foreach (var tankModel in _tankModels)
                BattleTankModel.Delete(tankModel.Id);

            _tankModels.Clear();
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