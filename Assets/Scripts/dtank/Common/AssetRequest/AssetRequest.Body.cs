using UnityEngine;

namespace dtank
{
    /// <summary>
    /// BodyPrefabのAssetRequest基底
    /// </summary>
    public abstract class BodyPrefabAssetRequestBase : AssetRequest<GameObject>
    {
        protected BodyPrefabAssetRequestBase(string relativePath)
            : base($"BodyAssets/{relativePath}")
        {
        }
    }

    /// <summary>
    /// TankPrefabのAssetRequest
    /// </summary>
    public class TankPrefabAssetRequest : BodyPrefabAssetRequestBase {
        public TankPrefabAssetRequest(string assetKey)
            : base($"Tank/prfb_tank_{assetKey}.prefab") {
        }
    }
}