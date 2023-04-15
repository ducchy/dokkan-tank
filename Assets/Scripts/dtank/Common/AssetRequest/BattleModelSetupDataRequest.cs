using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Core;
using GameFramework.CoroutineSystems;
using UniRx;

namespace dtank
{
    public class BattleModelSetupDataRequest
    {
        public AsyncOperationHandle<BattleModelSetupData> LoadAsync(BattleEntryData entryData, IScope scope)
        {
            var op = new AsyncOperator<BattleModelSetupData>();

            var modelSetupData = default(BattleModelSetupData);
            LoadRoutine(entryData, data => modelSetupData = data, scope)
                .ToObservable()
                .Subscribe(
                    onNext: _ => { },
                    onCompleted: () => { op.Completed(modelSetupData); }
                );

            return op;
        }

        private IEnumerator LoadRoutine(BattleEntryData entryData, Action<BattleModelSetupData> onLoaded, IScope scope)
        {
            var tankModelSetupDataDict = new Dictionary<int, BattleTankModelSetupData>();
            var ruleData = default(BattleRuleData);

            var sources = entryData.Players
                .Select(player => LoadTankModelSetupDataRoutine(player, tankModelSetupDataDict, scope).ToObservable())
                .ToList();
            sources.Add(LoadRuleModelObservable(entryData.RuleId, scope)
                .Do(data => ruleData = data)
                .AsUnitObservable());

            yield return sources.Merge()
                .StartAsEnumerator(scope);

            var modelSetupData = new BattleModelSetupData(ruleData, tankModelSetupDataDict);
            onLoaded?.Invoke(modelSetupData);
        }

        private IEnumerator LoadTankModelSetupDataRoutine(BattlePlayerEntryData player,
            Dictionary<int, BattleTankModelSetupData> dict, IScope scope)
        {
            var parameterData = default(BattleTankParameterData);
            yield return LoadTankParameterDataObservable(player.ParameterId, scope)
                .Do(x => parameterData = x)
                .StartAsEnumerator(scope);

            var actorSetupData = default(BattleTankActorSetupData);
            yield return LoadTankActorSetupDataObservable(parameterData.ActorSetupDataId, scope)
                .Do(x => actorSetupData = x)
                .StartAsEnumerator(scope);

            var tankModelSetupData = new BattleTankModelSetupData(parameterData, actorSetupData);
            dict.Add(player.PlayerId, tankModelSetupData);
        }

        private IObservable<BattleRuleData> LoadRuleModelObservable(int ruleId, IScope scope)
        {
            return new BattleRuleDataAssetRequest($"{ruleId:d3}")
                .LoadAsync(scope);
        }

        private IObservable<BattleTankParameterData> LoadTankParameterDataObservable(int parameterId, IScope scope)
        {
            return new BattleTankParameterDataAssetRequest($"{parameterId:d3}")
                .LoadAsync(scope);
        }

        private IObservable<BattleTankActorSetupData> LoadTankActorSetupDataObservable(int actorSetupDataId,
            IScope scope)
        {
            return new BattleTankActorSetupDataAssetRequest($"{actorSetupDataId:d3}")
                .LoadAsync(scope);
        }
    }
}