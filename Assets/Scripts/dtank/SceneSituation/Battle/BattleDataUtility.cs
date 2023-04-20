using System.Linq;
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
            var createTankModelSetupDataTasks =
                UniTask.WhenAll(
                    entryData.Players
                        .Select(player => CreateTankModelSetupDataAsync(player, scope))
                        .ToArray());
            var loadRuleDataTask = new BattleRuleDataAssetRequest(entryData.RuleId)
                .LoadAsync(scope)
                .ToUniTask();
            
            var (tankModelSetupDataDictPairs, ruleData) =
                await UniTask.WhenAll(createTankModelSetupDataTasks, loadRuleDataTask);

            var tankModelSetupDataDict = tankModelSetupDataDictPairs
                .ToDictionary(
                    tankModelSetupDataDictPair => tankModelSetupDataDictPair.Item1,
                    tankModelSetupDataDictPair => tankModelSetupDataDictPair.Item2);

            return new BattleModelSetupData(ruleData, tankModelSetupDataDict);
        }

        /// <summary>BattleTankModelのSetupに必要なデータの作成</summary>
        private static async UniTask<(int, BattleTankModelSetupData)> CreateTankModelSetupDataAsync(
            BattlePlayerEntryData player, IScope scope)
        {
            var parameterData = await new BattleTankParameterDataAssetRequest(player.ParameterId)
                .LoadAsync(scope)
                .ToUniTask();

            var actorSetupData = await new BattleTankActorSetupDataAssetRequest(parameterData.ActorSetupDataId)
                .LoadAsync(scope)
                .ToUniTask();

            return (player.PlayerId, new BattleTankModelSetupData(parameterData, actorSetupData));
        }
    }
}