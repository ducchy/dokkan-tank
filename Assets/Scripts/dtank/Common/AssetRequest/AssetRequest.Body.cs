using UnityEngine;

namespace dtank
{
    /// <summary>
    /// BodyPrefabのAssetRequest基底
    /// </summary>
    public abstract class BodyPrefabAssetRequestBase : AssetRequest<GameObject>
    {
    }

    /// <summary>
    /// TankPrefabのAssetRequest
    /// </summary>
    public class TankPrefabAssetRequest : BodyPrefabAssetRequestBase
    {
        public override string Address => $"Assets/AddressableAssets/Body/Tank/prfb_tank_{_assetKey}.prefab";
        private readonly string _assetKey;

        public TankPrefabAssetRequest(string assetKey)
        {
            _assetKey = assetKey;
        }
    }
}