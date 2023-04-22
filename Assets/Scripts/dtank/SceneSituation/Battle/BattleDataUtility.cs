using System.Linq;
using Cysharp.Threading.Tasks;
using GameFramework.Core;

namespace dtank
{
    /// <summary>バトル関連データのユーティリティ</summary>
    public static class BattleDataUtility
    {
        /// <summary>BattleのSetupに必要なデータの作成</summary>
        public static async UniTask<BattleSetupData> CreateBattleSetupDataAsync(BattleEntryData entryData,
            FieldData fieldData, IScope scope)
        {
            var modelSetupData = await CreateBattleModelSetupDataAsync(entryData, scope);
            var actorSetupData = await CreateBattleActorSetupDataAsync(entryData, fieldData, modelSetupData, scope);
            return new BattleSetupData(modelSetupData, actorSetupData);
        }

        /// <summary>BattleModelのSetupに必要なデータの作成</summary>
        private static async UniTask<BattleModelSetupData> CreateBattleModelSetupDataAsync(
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

            return (player.PlayerId, new BattleTankModelSetupData(parameterData));
        }

        /// <summary>BattleActorのSetupに必要なデータの作成</summary>
        private static async UniTask<BattleActorSetupData> CreateBattleActorSetupDataAsync(
            BattleEntryData entryData, FieldData fieldData, BattleModelSetupData modelSetupData, IScope scope)
        {
            var tankActorSetupDataDictPairs = await UniTask.WhenAll(
                entryData.Players
                    .Select(player =>
                    {
                        var tankModelSetupData = modelSetupData.TankModelSetupDataDict[player.PlayerId];
                        var startPointData = fieldData.StartPointDataArray[player.PositionIndex];
                        return CreateTankActorSetupDataAsync(player, tankModelSetupData, startPointData, scope);
                    })
                    .ToArray()
            );

            var tankActorSetupDataDict = tankActorSetupDataDictPairs
                .ToDictionary(
                    tankModelSetupDataDictPair => tankModelSetupDataDictPair.Item1,
                    tankModelSetupDataDictPair => tankModelSetupDataDictPair.Item2);

            return new BattleActorSetupData(tankActorSetupDataDict);
        }

        /// <summary>BattleTankActorのSetupに必要なデータの作成</summary>
        private static async UniTask<(int, BattleTankActorSetupData)> CreateTankActorSetupDataAsync(
            BattlePlayerEntryData player, BattleTankModelSetupData modelSetupData, TransformData startPointData,
            IScope scope)
        {
            var actorSetupData =
                await new BattleTankActorSetupDataAssetRequest(modelSetupData.ParameterData.ActorSetupDataId)
                    .LoadAsync(scope)
                    .ToUniTask();

            actorSetupData.SetNonSerializeData(startPointData);

            return (player.PlayerId, actorSetupData);
        }
    }
}