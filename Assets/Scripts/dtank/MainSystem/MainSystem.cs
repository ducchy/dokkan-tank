using GameFramework.Core;
using GameFramework.SituationSystems;
using System.Collections;
using GameFramework.AssetSystems;
using GameFramework.TaskSystems;
using UnityEngine;

namespace dtank
{
    /// <summary>
    /// dtankのメインシステム
    /// </summary>
    public class MainSystem : GameFramework.Core.MainSystem
    {
        [SerializeField] private ServiceContainerInstaller _globalObject;

        private TaskRunner _taskRunner;
        private SceneSituationContainer _sceneSituationContainer;

        protected override IEnumerator RebootRoutineInternal(object[] args)
        {
            yield break;
        }

        protected override IEnumerator StartRoutineInternal(object[] args)
        {
            Debug.Log("[MainSystem] Begin StartRoutineInternal()");

            // GlobalObjectを初期化
            DontDestroyOnLoad(_globalObject.gameObject);
            // RootのServiceにインスタンスを登録
            _globalObject.Install(Services.Instance);

            // 各種システム初期化
            _taskRunner = new TaskRunner();
            Services.Instance.Set(_taskRunner);
            var assetManager = new AssetManager();
            assetManager.Initialize(
                new AssetDatabaseAssetProvider(),
                new ResourcesAssetProvider());
            Services.Instance.Set(assetManager);

            // 各種GlobalObjectのタスク登録
            _taskRunner.Register(Services.Get<FadeController>(), TaskOrder.UI);

            _sceneSituationContainer = new SceneSituationContainer();
            _taskRunner.Register(_sceneSituationContainer, TaskOrder.PreSystem);

            var fieldManager = new FieldManager(Services.Instance);
            Services.Instance.Set(fieldManager);

#if DEVELOPMENT_BUILD || UNITY_EDITOR
            var debugManager = new DebugManager();
            Services.Instance.Set(debugManager);
            
            DebugManager.ServiceContainer.Set(_sceneSituationContainer);
#endif

            SceneSituation startSituation = null;
            if (args.Length > 0)
                startSituation = args[0] as SceneSituation;

            startSituation ??= new TitleSceneSituation();

            var handle = _sceneSituationContainer.Transition(startSituation, new CommonFadeTransitionEffect(0f, 0.5f));

            yield return handle;

            Debug.Log("[MainSystem] End StartRoutineInternal()");
        }

        protected override void UpdateInternal()
        {
            _taskRunner.Update();
        }

        protected override void LateUpdateInternal()
        {
            _taskRunner.LateUpdate();
        }

        protected override void OnDestroyInternal()
        {
            Debug.Log("[MainSystem] OnDestroyInternal()");

            _sceneSituationContainer.Dispose();
            _sceneSituationContainer = null;
        }
    }
}