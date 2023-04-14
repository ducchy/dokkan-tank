using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework.Core;
using GameFramework.CoroutineSystems;
using UniRx;

namespace dtank
{
    public class BattleModelSetUpDataRequester
    {
        public IEnumerator LoadRoutine(BattleEntryData entryData, Action<BattleModelSetUpData> onLoaded, IScope scope)
        {
            var tankModelSetupDataDict = new Dictionary<int, BattleTankModelSetupData>();
            foreach (var player in entryData.Players)
                yield return LoadTankModelSetupDataRoutine(player,
                    tankModelSetupData => tankModelSetupDataDict.Add(player.PlayerId, tankModelSetupData), scope);

            var ruleData = default(BattleRuleData);
            yield return new BattleRuleDataAssetRequest($"{entryData.RuleId:d3}")
                .LoadAsync(scope)
                .Do(x => ruleData = x)
                .StartAsEnumerator(scope);

            var modelSetupData = new BattleModelSetUpData(ruleData, tankModelSetupDataDict);
            onLoaded?.Invoke(modelSetupData);
        }

        private IEnumerator LoadTankModelSetupDataRoutine(BattlePlayerEntryData player,
            Action<BattleTankModelSetupData> onLoaded, IScope scope)
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
            onLoaded?.Invoke(tankModelSetupData);
        }
    }
}