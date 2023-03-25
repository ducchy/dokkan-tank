using System.Collections;
using System.Linq;
using GameFramework.Core;
using GameFramework.SituationSystems;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace dtank
{
    public class Field
    {
        public Scene Scene { get; private set; }
        public readonly int FieldId;
        
        private string SceneAssetPath => $"field{FieldId:d3}";

        public Field(int fieldId)
        {
            FieldId = fieldId;
        }

        public IEnumerator LoadRoutine(IServiceContainer container)
        {
            yield return SceneManager.LoadSceneAsync(SceneAssetPath, LoadSceneMode.Additive);

            Scene = SceneManager.GetSceneByName(SceneAssetPath);

            if (Scene.path != SceneAssetPath && Scene.name != SceneAssetPath)
            {
                Debug.LogError($"Failed load scene. [{SceneAssetPath}]");
            }

            var installers = Scene.GetRootGameObjects()
                .SelectMany(x => x.GetComponentsInChildren<ServiceContainerInstaller>(true))
                .ToArray();
            
            foreach (var installer in installers)
                installer.Install(container);

            var fieldView = container.Get<FieldView>();
            if (fieldView != null)
                container.Set(fieldView.CreateData());
        }

        public void UnloadRoutine()
        {
            SceneManager.UnloadSceneAsync(SceneAssetPath);
        }
    }
}
