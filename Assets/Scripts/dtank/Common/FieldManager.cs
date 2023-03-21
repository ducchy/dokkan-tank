using System.Collections;
using GameFramework.Core;
using UnityEngine;

namespace dtank
{
    public class FieldManager
    {
        public readonly ServiceContainer ServiceContainer;
        
        private Field _currentField = null;

        public FieldManager(IServiceContainer parent)
        {
            ServiceContainer = new ServiceContainer(parent);
        }

        public IEnumerator LoadRoutine(int id)
        {
            if (_currentField != null && _currentField.FieldId == id)
                yield break;
                
            Debug.Log("Begin FieldManager.LoadRoutine()");

            _currentField = new Field(id);
            yield return _currentField.LoadRoutine(ServiceContainer);
            
            Debug.Log("End FieldManager.LoadRoutine()");
        }

        public void Unload()
        {
            if (_currentField == null)
                return;
            
            _currentField.UnloadRoutine();
            _currentField = null;
            ServiceContainer.Dispose();
            
            Debug.Log("FieldManager.Unload()");
        }
    }
}