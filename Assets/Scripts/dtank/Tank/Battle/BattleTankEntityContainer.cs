using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework.Core;
using GameFramework.CoroutineSystems;
using GameFramework.EntitySystems;
using UnityEngine;

namespace dtank
{
    public class BattleTankEntityContainer : IDisposable
    {
        private readonly Dictionary<int, Entity> _dictionary = new Dictionary<int, Entity>();
        public IReadOnlyDictionary<int, Entity> Dictionary => _dictionary;

        public void Dispose()
        {
            if (_dictionary == null)
                return;

            foreach (var battleTankEntity in _dictionary.Values)
                battleTankEntity.Dispose();

            _dictionary.Clear();
        }

        public IEnumerator AddRoutine(BattleTankModel model, IScope scope)
        {
            if (_dictionary.ContainsKey(model.Id))
            {
                Debug.Log($"IDが重複: id={model.Id}");
                yield break;
            }

            var entity = new Entity();
            _dictionary.Add(model.Id, entity);

            yield return entity.SetupBattleTankAsync(model, scope)
                .StartAsEnumerator(scope);
        }

        public Entity Get(int id)
        {
            return _dictionary.TryGetValue(id, out var value) ? value : null;
        }
    }
}