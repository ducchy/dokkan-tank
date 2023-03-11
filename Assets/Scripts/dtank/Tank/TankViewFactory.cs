using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace dtank
{
    public abstract class TankViewFactory<TTankView> : IDisposable
        where TTankView : TankViewBase
    {
        private Dictionary<int, TTankView> _prefabDictionary = new Dictionary<int, TTankView>();

        public void Dispose()
        {
            _prefabDictionary.Clear();
        }

        private TTankView GetPrefab(int id)
        {
            if (_prefabDictionary.TryGetValue(id, out var prefab))
                return prefab;

            prefab = LoadPrefab(id);
            if (prefab == null)
                return null;

            _prefabDictionary.Add(id, prefab);
            return prefab;
        }

        private TTankView LoadPrefab(int id)
        {
            return Resources.Load<TTankView>(CreatePrefabPath(id));
        }

        public TTankView Create(int id)
        {
            var prefab = GetPrefab(id);
            if (prefab == null)
            {
                Debug.LogErrorFormat("Prefabのロード失敗: id={0}", id);
                return null;
            }

            var instance = Object.Instantiate(prefab);
            OnCreated(instance);
            return instance;
        }

        protected abstract string CreatePrefabPath(int id);

        protected abstract void OnCreated(TTankView view);
    }
}