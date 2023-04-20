using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameFramework.Core;

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
                tankModelSetupDataDict.Add(player.PlayerId, await CreateTankModelSetupDataAsync(player, scope));

            var ruleData = await new BattleRuleDataAssetRequest(entryData.RuleId)
                .LoadAsync(scope)
                .ToUniTask();

            return new BattleModelSetupData(ruleData, tankModelSetupDataDict);
        }

        /// <summary>BattleTankModelのSetupに必要なデータの作成</summary>
        private static async UniTask<BattleTankModelSetupData> CreateTankModelSetupDataAsync(
            BattlePlayerEntryData player, IScope scope)
        {
            var parameterData = await new BattleTankParameterDataAssetRequest(player.ParameterId)
                .LoadAsync(scope)
                .ToUniTask();

            var actorSetupData = await new BattleTankActorSetupDataAssetRequest(parameterData.ActorSetupDataId)
                .LoadAsync(scope)
                .ToUniTask();

            return new BattleTankModelSetupData(parameterData, actorSetupData);
        }
    }
}