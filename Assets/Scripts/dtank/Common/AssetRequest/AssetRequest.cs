using System;
using System.Collections.Generic;
using GameFramework.AssetSystems;
using GameFramework.Core;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace dtank
{
    /// <summary>
    /// Sample用のAssetRequest基底
    /// </summary>
    public abstract class ResourceAssetRequest<T> : GameFramework.AssetSystems.AssetRequest<T>
        where T : Object
    {
        public override string Address { get; }
        public override int[] ProviderIndices => new[] { (int)AssetProviderType.Resources };

        protected ResourceAssetRequest(string relativePath)
        {
            Address = GetPath(relativePath);
        }

        /// <summary>
        /// アセットの読み込み
        /// </summary>
        /// <param name="unloadScope">解放スコープ</param>
        public IObservable<T> LoadAsync(IScope unloadScope)
        {
            return Observable.Create<T>(observer =>
            {
                var handle = LoadAsync(Services.Get<AssetManager>(), unloadScope);
                if (!handle.IsValid)
                {
                    observer.OnError(new KeyNotFoundException($"Load failed. {Address}"));
                    return Disposable.Empty;
                }

                if (handle.IsDone)
                {
                    observer.OnNext(handle.Asset);
                    observer.OnCompleted();
                    return Disposable.Empty;
                }

                // 読み込みを待つ
                return Observable.EveryUpdate()
                    .Subscribe(_ =>
                    {
                        if (handle.Exception != null)
                        {
                            observer.OnError(handle.Exception);
                        }
                        else if (handle.IsDone)
                        {
                            observer.OnNext(handle.Asset);
                            observer.OnCompleted();
                        }
                    });
            });
        }

        private string GetPath(string relativePath)
        {
            return $"Assets/SampleGame/Resources/{relativePath}";
        }
    }

    #region DataAsset

    /// <summary>
    /// DataのAssetRequest基底
    /// </summary>
    public abstract class DataAssetRequestBase<T> : ResourceAssetRequest<T>
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

    #endregion DataAsset

    #region Body

    /// <summary>
    /// BodyPrefabのAssetRequest基底
    /// </summary>
    public abstract class BodyPrefabAssetRequestBase : ResourceAssetRequest<GameObject>
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

    #endregion Body
}