using GameFramework.Core;
using GameFramework.SituationSystems;
using System.Collections;
using UnityEngine;

namespace dtank
{
    /// <summary>
    /// dtankのメインシステム
    /// </summary>
    public class MainSystem : GameFramework.Core.MainSystem
    {
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

            _sceneSituationContainer = new SceneSituationContainer();
            Services.Instance.Set(_sceneSituationContainer);

            SceneSituation startSituation = null;
            if (args.Length > 0)
                startSituation = args[0] as SceneSituation;

            if (startSituation == null)
                startSituation = new TitleSceneSituation();

            var handle = _sceneSituationContainer.Transition(startSituation);

            yield return handle;

            Debug.Log("End StartRoutineInternal()");
        }

        protected override void UpdateInternal()
        {
            _sceneSituationContainer.Update();
        }

        protected override void LateUpdateInternal()
        {
            _sceneSituationContainer.LateUpdate();
        }

        protected override void OnDestroyInternal()
        {
            Debug.Log("OnDestroyInternal()");

            _sceneSituationContainer.Dispose();
            _sceneSituationContainer = null;
        }
    }
}