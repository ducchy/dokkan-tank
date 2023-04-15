using System.Collections;
using GameFramework.AssetSystems;
using GameFramework.Core;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace dtank
{
    public class Workspace : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            //StartCoroutine(CreateTankRoutine());
            StartCoroutine(CreateTankRoutine2());
        }

        private IEnumerator CreateTankRoutine()
        {
            var handle =
                Addressables.LoadAssetAsync<GameObject>("Assets/AddressableAssets/Body/Tank/prfb_tank_001.prefab");
            yield return handle;
            Instantiate(handle.Result);
        }

        private IEnumerator CreateTankRoutine2()
        {
            var assetManager = new AssetManager();
            assetManager.Initialize(new AddressablesAssetProvider());

            var addressableInitializeHandle = Addressables.InitializeAsync();
            yield return addressableInitializeHandle;
            
            var request = new TankPrefabAssetRequest("001");

            IScope loadScope = new DisposableScope();
            var handle = request.LoadAsync(assetManager, loadScope);
            yield return handle;

            Instantiate(handle.Asset);
        }
    }
}