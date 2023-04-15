using System.Collections.Generic;
using GameFramework.Core;
using GameFramework.CoroutineSystems;
using GameFramework.ModelSystems;
using GameFramework.TaskSystems;
using UniRx;
using UnityEngine;

namespace dtank
{
    public class BattleModel : SingleModel<BattleModel>, ITask, ITaskEventHandler
    {
        private readonly ReactiveProperty<BattleState> _currentState = new();
        public IReadOnlyReactiveProperty<BattleState> CurrentState => _currentState;

        private readonly List<BattleTankModel> _tankModels = new();
        public IReadOnlyList<BattleTankModel> TankModels => _tankModels;

        private readonly List<NpcBehaviourSelector> _npcBehaviourSelectors = new();
        public IReadOnlyList<NpcBehaviourSelector> NpcBehaviourSelectors => _npcBehaviourSelectors;

        public BattleTankModel MainPlayerTankModel { get; private set; }
        public BattleRuleModel RuleModel { get; private set; }

        private readonly CoroutineRunner _coroutineRunner = new();
        private TaskRunner _taskRunner;

        bool ITask.IsActive => true;

        private BattleModel(object empty) : base(empty)
        {
        }

        public void Setup(BattleEntryData entryData, FieldViewData fieldViewData, BattleModelSetupData setupData)
        {
            _tankModels.Clear();
            foreach (var player in entryData.Players)
            {
                var tankModelSetupData = setupData.TankModelSetupDataDict[player.PlayerId];

                var tankModel = BattleTankModel.Create();
                tankModel.Setup(player.Name, player.BodyId, player.CharacterType, tankModelSetupData.ParameterData);

                var startPointData = fieldViewData.StartPointDataArray[player.PositionIndex];
                tankModel.ActorModel.Update(tankModelSetupData.ActorSetupData, startPointData);

                _tankModels.Add(tankModel);

                if (player.PlayerId == entryData.MainPlayerId)
                    MainPlayerTankModel = tankModel;
            }

            _npcBehaviourSelectors.Clear();
            foreach (var tankModel in _tankModels)
                _npcBehaviourSelectors.Add(new NpcBehaviourSelector(tankModel, _tankModels));

            var ruleData = setupData.RuleData;
            var mainPlayerId = MainPlayerTankModel.Id;
            RuleModel = new BattleRuleModel(ruleData.Duration, mainPlayerId, _tankModels);

            Bind();
        }

        protected override void OnDeletedInternal()
        {
            Debug.Log("[BattleModel] OnDeletedInternal");

            base.OnDeletedInternal();

            RuleModel?.Dispose();
            foreach (var tankModel in _tankModels)
                BattleTankModel.Delete(tankModel.Id);

            _taskRunner?.Unregister(this);

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

            var deltaTime = Time.deltaTime;
            RuleModel?.Update(deltaTime);
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

        void ITaskEventHandler.OnRegistered(TaskRunner runner)
        {
            Debug.Log("[BattleModel] OnRegistered");

            _taskRunner = runner;
        }

        void ITaskEventHandler.OnUnregistered(TaskRunner runner)
        {
            Debug.Log("[BattleModel] OnUnregistered");

            if (_taskRunner == runner)
                _taskRunner = null;
        }
    }
}