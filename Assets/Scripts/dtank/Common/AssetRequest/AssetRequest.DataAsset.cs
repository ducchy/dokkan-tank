using UnityEngine;

namespace dtank
{
    public abstract class DataAssetRequestBase<T> : AssetRequest<T>
        where T : Object
    {
        protected DataAssetRequestBase(string relativePath)
            : base($"Data/{relativePath}")
        {
        }
    }

    public abstract class BattleDataAssetRequestBase<T> : DataAssetRequestBase<T>
        where T : Object
    {
        protected BattleDataAssetRequestBase(string relativePath)
            : base($"Battle/{relativePath}")
        {
        }
    }

    public class BattleRuleDataAssetRequest : BattleDataAssetRequestBase<BattleRuleData>
    {
        public BattleRuleDataAssetRequest(string assetKey)
            : base($"BattleRule/dat_rule_{assetKey}.asset")
        {
        }
    }

    public class BattleTankParameterDataAssetRequest : BattleDataAssetRequestBase<BattleTankParameterData>
    {
        public BattleTankParameterDataAssetRequest(string assetKey)
            : base($"TankParameter/dat_tank_parameter_{assetKey}.asset")
        {
        }
    }

    public class BattleTankActorSetupDataAssetRequest : BattleDataAssetRequestBase<BattleTankActorSetupData>
    {
        public BattleTankActorSetupDataAssetRequest(string assetKey)
            : base($"TankActorSetup/dat_tank_actor_setup_{assetKey}.asset")
        {
        }
    }
}