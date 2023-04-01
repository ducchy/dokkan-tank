using System;
using System.Collections.Generic;
using GameFramework.BodySystems;
using GameFramework.EntitySystems;
using UniRx;

namespace dtank
{
    /// <summary>
    /// エンティティ用のUtility
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// 基本的なEntityの初期化処理
        /// </summary>
        /// <param name="source">初期化対象のEntity</param>
        /// <param name="onCreateBody">Body生成</param>
        /// <param name="onSetupEntity">各種初期化処理</param>
        public static IObservable<Entity> SetupAsync(this Entity source, Func<IObservable<Body>> onCreateBody,
            Func<Entity, IObservable<Unit>> onSetupEntity)
        {
            return Observable.Defer(() =>
            {
                // Entityの初期化
                var bodyEntityComponent = source.AddOrGetComponent<BodyEntityComponent>();
                source.AddOrGetComponent<LogicEntityComponent>();
                source.AddOrGetComponent<ActorEntityComponent>();
                source.AddOrGetComponent<ModelEntityComponent>();

                var streams = new List<IObservable<Unit>>();

                // Bodyの生成
                if (onCreateBody != null)
                {
                    streams.Add(Observable.Defer(onCreateBody.Invoke)
                        .Do(body => bodyEntityComponent.SetBody(body))
                        .AsUnitObservable()
                    );
                }

                // Body生成後の初期化
                if (onSetupEntity != null)
                    streams.Add(Observable.Defer(() => onSetupEntity.Invoke(source)));

                return streams.Concat()
                    .AsSingleUnitObservable()
                    .Select(_ => source);
            });
        }
    }
}