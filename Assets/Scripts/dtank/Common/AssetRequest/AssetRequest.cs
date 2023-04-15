using System;
using System.Collections.Generic;
using GameFramework.AssetSystems;
using GameFramework.Core;
using UniRx;
using Object = UnityEngine.Object;

namespace dtank
{
    /// <summary>
    /// Sample用のAssetRequest基底
    /// </summary>
    public abstract class AssetRequest<T> : GameFramework.AssetSystems.AssetRequest<T>
        where T : Object
    {
        public override int[] ProviderIndices =>
            new[] { (int)AssetProviderType.AssetDatabase };

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
    }
}