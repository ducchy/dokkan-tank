using System.Collections;
using GameFramework.Core;
using UnityEngine;

namespace dtank
{
    public class FieldManager
    {
        private readonly ServiceContainer _serviceContainer;
        
        private Field _currentField;

        public FieldManager(IServiceContainer parent)
        {
            _serviceContainer = new ServiceContainer(parent);
        }

        public IEnumerator LoadRoutine(int id)
        {
            if (_currentField != null && _currentField.FieldId == id)
                yield break;
            
            Debug.Log($"[FieldManager] Load: id={id}");
                
            _currentField = new Field(id);
            yield return _currentField.LoadRoutine(_serviceContainer);
        }

        public void Unload()
        {
            if (_currentField == null)
                return;
            
            Debug.Log("[FieldManager] Unload");
            
            _currentField.UnloadRoutine();
            _currentField = null;
            _serviceContainer.Dispose();
        }
    }
}