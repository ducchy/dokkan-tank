using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Core;
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
                    CreateTankModelSetupDataAsync(player, scope)
                        .Do(data => tankModelSetupDataDict.Add(player.PlayerId, data))
                        .AsUnitObservable()
                )
                .ToList();

            var ruleData = default(BattleRuleData);
            loadObservables.Add(new BattleRuleDataAssetRequest(entryData.RuleId)
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
        private static IObservable<BattleTankModelSetupData> CreateTankModelSetupDataAsync(BattlePlayerEntryData player,
            IScope scope)
        {
            return new BattleTankParameterDataAssetRequest(player.ParameterId)
                .LoadAsync(scope)
                .SelectMany(parameterData =>
                    new BattleTankActorSetupDataAssetRequest(parameterData.ActorSetupDataId)
                        .LoadAsync(scope)
                        .Select(actorSetupData => new BattleTankModelSetupData(parameterData, actorSetupData)));
        }
    }
}