using UnityEngine;

namespace dtank
{
    public abstract class DataAssetRequestBase<T> : AssetRequest<T>
        where T : Object
    {
    }

    public abstract class BattleDataAssetRequestBase<T> : DataAssetRequestBase<T>
        where T : Object
    {
    }

    public class BattleRuleDataAssetRequest : BattleDataAssetRequestBase<BattleRuleData>
    {
        public override string Address => $"Assets/AddressableAssets/Data/Battle/BattleRule/dat_rule_{_assetKey}.asset";
        private readonly string _assetKey;

        public BattleRuleDataAssetRequest(int ruleId)
        {
            _assetKey = $"{ruleId:d3}";
        }
    }

    public class BattleTankParameterDataAssetRequest : BattleDataAssetRequestBase<BattleTankParameterData>
    {
        public override string Address =>
            $"Assets/AddressableAssets/Data/Battle/TankParameter/dat_tank_parameter_{_assetKey}.asset";

        private readonly string _assetKey;

        public BattleTankParameterDataAssetRequest(int parameterId)
        {
            _assetKey = $"{parameterId:d3}";
        }
    }

    public class BattleTankActorSetupDataAssetRequest : BattleDataAssetRequestBase<BattleTankActorSetupData>
    {
        public override string Address =>
            $"Assets/AddressableAssets/Data/Battle/TankActorSetup/dat_tank_actor_setup_{_assetKey}.asset";

        private readonly string _assetKey;

        public BattleTankActorSetupDataAssetRequest(int actorSetupDataId)
        {
            _assetKey = $"{actorSetupDataId:d3}";
        }
    }
}