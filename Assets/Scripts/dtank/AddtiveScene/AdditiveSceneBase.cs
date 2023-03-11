using System.Collections;
using System.Linq;
using GameFramework.Core;
using GameFramework.SituationSystems;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace dtank
{
    public abstract class AdditiveSceneBase
    {
        protected abstract string SceneAssetPath { get; }

        public Scene Scene { get; private set; }

        public IEnumerator LoadRoutine(ServiceContainer mainSceneServiceContainer)
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
            {
                installer.Install(mainSceneServiceContainer);
            }
        }
    }
}