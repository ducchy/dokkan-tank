using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace dtank
{
    public abstract class TankActorFactory<TTankActor> : IDisposable
        where TTankActor : TankActorBase
    {
        private readonly Dictionary<int, TTankActor> _prefabDictionary = new Dictionary<int, TTankActor>();

        public void Dispose()
        {
            _prefabDictionary.Clear();
        }

        private TTankActor GetPrefab(int id)
        {
            if (_prefabDictionary.TryGetValue(id, out var prefab))
                return prefab;

            prefab = LoadPrefab(id);
            if (prefab == null)
                return null;

            _prefabDictionary.Add(id, prefab);
            return prefab;
        }

        private TTankActor LoadPrefab(int id)
        {
            return Resources.Load<TTankActor>(CreatePrefabPath(id));
        }

        public TTankActor Create(int id, Transform parent)
        {
            var prefab = GetPrefab(id);
            if (prefab == null)
            {
                Debug.LogErrorFormat("Prefabのロード失敗: id={0}", id);
                return null;
            }

            var instance = Object.Instantiate(prefab, parent);
            OnCreated(instance);
            return instance;
        }

        protected abstract string CreatePrefabPath(int id);

        protected abstract void OnCreated(TTankActor actor);
    }
}