using GameFramework.Core;
using GameFramework.SituationSystems;
using System.Collections;
using GameFramework.TaskSystems;
using UnityEngine;

namespace dtank
{
    /// <summary>
    /// dtankのメインシステム
    /// </summary>
    public class MainSystem : GameFramework.Core.MainSystem
    {
        [SerializeField]
        private ServiceContainerInstaller _globalObject;
        
        private TaskRunner _taskRunner;
        private SceneSituationContainer _sceneSituationContainer;

        protected override IEnumerator RebootRoutineInternal(object[] args)
        {
            Debug.Log("Begin RebootRoutineInternal()");

            Debug.Log("End RebootRoutineInternal()");

            yield break;
        }

        protected override IEnumerator StartRoutineInternal(object[] args)
        {
            Debug.Log("Begin StartRoutineInternal()");
            
            // GlobalObjectを初期化
            DontDestroyOnLoad(_globalObject.gameObject);
            // RootのServiceにインスタンスを登録
            _globalObject.Install(Services.Instance);

            // 各種システム初期化
            _taskRunner = new TaskRunner();
            Services.Instance.Set(_taskRunner);

            // 各種GlobalObjectのタスク登録
            _taskRunner.Register(Services.Get<FadeController>(), TaskOrder.UI);
            
            _sceneSituationContainer = new SceneSituationContainer();
            _taskRunner.Register(_sceneSituationContainer, TaskOrder.PreSystem);

            SceneSituation startSituation = null;
            if (args.Length > 0)
                startSituation = args[0] as SceneSituation;

            if (startSituation == null)
                startSituation = new TitleSceneSituation();

            var handle = _sceneSituationContainer.Transition(startSituation, new CommonFadeTransitionEffect());

            yield return handle;

            Debug.Log("End StartRoutineInternal()");
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
            Debug.Log("OnDestroyInternal()");

            _sceneSituationContainer.Dispose();
            _sceneSituationContainer = null;
        }
    }
}