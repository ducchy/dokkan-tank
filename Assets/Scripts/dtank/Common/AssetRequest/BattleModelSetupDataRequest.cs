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
            sources.Add(LoadRuleModelRoutine(entryData.RuleId, data => ruleData = data, scope).ToObservable());

            yield return sources.Merge()
                .StartAsEnumerator(scope);

            var modelSetupData = new BattleModelSetupData(ruleData, tankModelSetupDataDict);
            onLoaded?.Invoke(modelSetupData);
        }

        private IEnumerator LoadRuleModelRoutine(int ruleId, Action<BattleRuleData> onLoaded, IScope scope)
        {
            var ruleData = default(BattleRuleData);
            yield return new BattleRuleDataAssetRequest($"{ruleId:d3}")
                .LoadAsync(scope)
                .Do(x => ruleData = x)
                .StartAsEnumerator(scope);

            onLoaded?.Invoke(ruleData);
        }

        private IEnumerator LoadTankModelSetupDataRoutine(BattlePlayerEntryData player,
            Dictionary<int, BattleTankModelSetupData> dict, IScope scope)
        {
            var parameterData = default(BattleTankParameterData);
            yield return new BattleTankParameterDataAssetRequest($"{player.ParameterId:d3}")
                .LoadAsync(scope)
                .Do(x => parameterData = x)
                .StartAsEnumerator(scope);

            var actorSetupData = default(BattleTankActorSetupData);
            yield return new BattleTankActorSetupDataAssetRequest($"{parameterData.ActorSetupDataId:d3}")
                .LoadAsync(scope)
                .Do(x => actorSetupData = x)
                .StartAsEnumerator(scope);

            var tankModelSetupData = new BattleTankModelSetupData(parameterData, actorSetupData);
            dict.Add(player.PlayerId, tankModelSetupData);
        }
    }
}