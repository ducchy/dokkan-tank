using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameFramework.Core;
using UniRx;

namespace dtank
{
    /// <summary>バトル関連データのユーティリティ</summary>
    public static class BattleDataUtility
    {
        /// <summary>BattleModelのSetupに必要なデータの作成</summary>
        public static async UniTask<BattleModelSetupData> CreateBattleModelSetupDataAsync(
            BattleEntryData entryData, IScope scope)
        {
            var tankModelSetupDataDict = new Dictionary<int, BattleTankModelSetupData>();
            foreach (var player in entryData.Players)
            {
                var tankModelSetupData = await CreateTankModelSetupDataAsync(player, scope);
                tankModelSetupDataDict.Add(player.PlayerId, tankModelSetupData);
            }
            
            var ruleData = default(BattleRuleData);
            await new BattleRuleDataAssetRequest(entryData.RuleId)
                .LoadAsync(scope)
                .Do(data => ruleData = data)
                .ToUniTask();

            return new BattleModelSetupData(ruleData, tankModelSetupDataDict);
        }

        /// <summary>BattleTankModelのSetupに必要なデータの作成</summary>
        private static async UniTask<BattleTankModelSetupData> CreateTankModelSetupDataAsync(
            BattlePlayerEntryData player, IScope scope)
        {
            var parameterData = default(BattleTankParameterData);
            await new BattleTankParameterDataAssetRequest(player.ParameterId)
                .LoadAsync(scope)
                .Do(data => parameterData = data)
                .ToUniTask();

            var actorSetupData = default(BattleTankActorSetupData);
            await new BattleTankActorSetupDataAssetRequest(parameterData.ActorSetupDataId)
                .LoadAsync(scope)
                .Do(data => actorSetupData = data)
                .ToUniTask();

            return new BattleTankModelSetupData(parameterData, actorSetupData);
        }
    }
}