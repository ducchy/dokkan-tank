using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Core;
using GameFramework.CoroutineSystems;
using UniRx;

namespace dtank
{
    /// <summary>バトル関連データのユーティリティ</summary>
    public static class BattleDataUtility
    {
        /// <summary>BattleModelのSetupに必要なデータの作成</summary>
        public static AsyncOperationHandle<BattleModelSetupData> CreateBattleModelSetupDataAsync(
            BattleEntryData entryData, IScope scope)
        {
            var op = new AsyncOperator<BattleModelSetupData>();

            var tankModelSetupDataDict = new Dictionary<int, BattleTankModelSetupData>();
            var loadObservables = entryData.Players
                .Select(player =>
                    CreateTankModelSetupDataAsync(
                        player,
                        data => tankModelSetupDataDict.Add(player.PlayerId, data),
                        scope).ToObservable())
                .ToList();

            var ruleData = default(BattleRuleData);
            loadObservables.Add(new BattleRuleDataAssetRequest($"{entryData.RuleId:d3}")
                .LoadAsync(scope)
                .Do(data => ruleData = data)
                .AsUnitObservable());

            var modelSetupData = default(BattleModelSetupData);
            loadObservables.WhenAll()
                .Do(_ => modelSetupData = new BattleModelSetupData(ruleData, tankModelSetupDataDict))
                .Subscribe(
                    onNext: _ => { },
                    onCompleted: () => { op.Completed(modelSetupData); }
                );

            return op;
        }

        /// <summary>BattleTankModelのSetupに必要なデータの作成</summary>
        private static IEnumerator CreateTankModelSetupDataAsync(BattlePlayerEntryData player,
            Action<BattleTankModelSetupData> onCreated, IScope scope)
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
            onCreated?.Invoke(tankModelSetupData);
        }
    }
}