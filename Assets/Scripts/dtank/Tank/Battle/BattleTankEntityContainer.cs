using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameFramework.BodySystems;
using GameFramework.Core;
using GameFramework.CoroutineSystems;
using GameFramework.EntitySystems;
using GameFramework.TaskSystems;
using UniRx;
using UnityEngine;

namespace dtank
{
    public class BattleTankEntityContainer : IDisposable
    {
        private readonly Dictionary<int, Entity> _dictionary = new();

        private readonly BattleTankControlUiView _controlUiView;
        private readonly BattlePlayerStatusUiView _statusUiView;

        public BattleTankEntityContainer(BattleTankControlUiView controlUiView, BattlePlayerStatusUiView statusUiView)
        {
            _controlUiView = controlUiView;
            _statusUiView = statusUiView;
        }

        public void Dispose()
        {
            if (_dictionary == null)
                return;

            foreach (var battleTankEntity in _dictionary.Values)
                battleTankEntity.Dispose();

            _dictionary.Clear();
        }

        public IEnumerator SetupRoutine(
            IReadOnlyList<BattleTankModel> tankModels,
            IReadOnlyList<NpcBehaviourSelector> npcBehaviourSelectors, 
            IScope scope)
        {
            Dispose();

            foreach (var tankModel in tankModels)
            {
                var npcBehaviourSelector = npcBehaviourSelectors.FirstOrDefault(nbs => nbs.OwnerId == tankModel.Id);
                yield return AddRoutine(tankModel, npcBehaviourSelector, tankModels, scope);
            }
        }

        private IEnumerator AddRoutine(
            BattleTankModel model, 
            NpcBehaviourSelector npcBehaviourSelector,
            IReadOnlyList<BattleTankModel> tankModels, 
            IScope scope)
        {
            if (_dictionary.ContainsKey(model.Id))
            {
                Debug.Log($"IDが重複: id={model.Id}");
                yield break;
            }

            var entity = new Entity();
            _dictionary.Add(model.Id, entity);

            yield return SetupBattleTankAsync(entity, model, npcBehaviourSelector, tankModels, scope)
                .StartAsEnumerator(scope);
        }

        public Entity Get(int id)
        {
            return _dictionary.TryGetValue(id, out var value) ? value : null;
        }

        /// <summary>
        /// BattleTankEntityの初期化処理
        /// </summary>
        private IObservable<Entity> SetupBattleTankAsync(
            Entity source, 
            BattleTankModel model,
            NpcBehaviourSelector npcBehaviourSelector,
            IReadOnlyList<BattleTankModel> tankModels, 
            IScope scope)
        {
            return source.SetupAsync(() =>
            {
                return new TankPrefabAssetRequest(model.AssetKey)
                    .LoadAsync(scope)
                    .Select(prefab => Services.Get<BodyManager>().CreateFromPrefab(prefab));
            }, entity =>
            {
                var taskRunner = Services.Get<TaskRunner>();
                var body = entity.GetBody();
                body.UserId = model.Id;
                var actor = new BattleTankActor(body, model.ActorModel.SetupData,
                    model.ActorModel.StartPointData);
                taskRunner.Register(actor, TaskOrder.Actor);
                var controlUiView = model.CharacterType == CharacterType.Player ? _controlUiView : null;
                var statusUiView = _statusUiView.GetStatusUi(model.Id);
                var logic = new BattleTankLogic(BattleModel.Get(), model, actor, controlUiView,
                    npcBehaviourSelector, statusUiView);
                taskRunner.Register(logic, TaskOrder.Logic);
                entity.AddActor(actor)
                    .AddLogic(logic);
                return Observable.ReturnUnit();
            });
        }
    }
}