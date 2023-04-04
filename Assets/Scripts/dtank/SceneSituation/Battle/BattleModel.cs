using System;
using System.Collections;
using System.Collections.Generic;
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
        private readonly ReactiveProperty<BattleState> _currentState = new();
        public IReadOnlyReactiveProperty<BattleState> CurrentState => _currentState;

        private readonly List<BattleTankModel> _tankModels = new();
        public IReadOnlyList<BattleTankModel> TankModels => _tankModels;

        public BattleTankModel MainPlayerTankModel { get; private set; }

        public BattleRuleModel RuleModel { get; private set; }

        private readonly CoroutineRunner _coroutineRunner = new();

        bool ITask.IsActive => true;

        private BattleModel(object empty) : base(empty)
        {
        }

        public IObservable<Unit> SetupAsync(BattleEntryData entryData, FieldViewData fieldViewData)
        {
            return Observable.Defer(() =>
            {
                var scope = new DisposableScope();
                return _coroutineRunner.StartCoroutineAsync(SetupRoutine(entryData, fieldViewData, scope),
                    () => { scope.Dispose(); });
            });
        }

        private IEnumerator SetupRoutine(BattleEntryData entryData, FieldViewData fieldViewData, IScope scope)
        {
            _tankModels.Clear();
            foreach (var player in entryData.Players)
            {
                var parameterData = default(BattleTankParameterData);
                yield return new BattleTankParameterDataAssetRequest($"{player.ParameterId:d3}")
                    .LoadAsync(scope)
                    .Do(x => parameterData = x)
                    .StartAsEnumerator(scope);

                var tankModel = BattleTankModel.Create();
                tankModel.Setup(player.Name, player.BodyId, player.CharacterType, parameterData);

                var actorSetupData = default(BattleTankActorSetupData);
                yield return new BattleTankActorSetupDataAssetRequest($"{parameterData.ActorSetupDataId:d3}")
                    .LoadAsync(scope)
                    .Do(x => actorSetupData = x)
                    .StartAsEnumerator(scope);
                var startPointData = fieldViewData.StartPointDataArray[player.PositionIndex];
                tankModel.ActorModel.Update(actorSetupData, startPointData);

                _tankModels.Add(tankModel);

                if (player.PlayerId == entryData.MainPlayer.PlayerId)
                    MainPlayerTankModel = tankModel;
            }

            var ruleData = default(BattleRuleData);
            yield return new BattleRuleDataAssetRequest($"{entryData.RuleId:d3}")
                .LoadAsync(scope)
                .Do(x => ruleData = x)
                .StartAsEnumerator(scope);

            var mainPlayerId = MainPlayerTankModel.Id;
            RuleModel = new BattleRuleModel(ruleData.Duration, mainPlayerId, _tankModels);

            Bind();
        }

        protected override void OnDeletedInternal()
        {
            base.OnDeletedInternal();

            RuleModel?.Dispose();
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
                tankModel.CurrentState
                    .Subscribe(state =>
                    {
                        if (state == BattleTankState.Dead)
                            RuleModel.Dead(tankModel.Id);
                    })
                    .ScopeTo(this);
                
                tankModel.Score
                    .Subscribe(_ => RuleModel.UpdateRanking())
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

        public void ChangeState(BattleState next)
        {
            var current = _currentState.Value;
            if (current == next)
                return;

            Debug.Log($"[BattleModel] ChangeState: {current} -> {next}");

            _currentState.Value = next;

            switch (next)
            {
                case BattleState.Ready:
                    foreach (var tankModel in _tankModels)
                        tankModel.ResetParameter();
                    RuleModel.Reset();
                    break;
                case BattleState.Playing:
                    RuleModel.Start();
                    break;
            }
        }
    }
}