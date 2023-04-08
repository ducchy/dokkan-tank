#if DEVELOPMENT_BUILD || UNITY_EDITOR

using System.Collections;
using GameFramework.SituationSystems;
using UnityDebugSheet.Runtime.Core.Scripts;

namespace dtank
{
    public sealed class DtankSceneDebugPage : DefaultDebugPageBase
    {
        protected override string Title => "dtank Debug > Scene";

        public override IEnumerator Initialize()
        {
            AddButton("Title", clicked: () => OnClickSceneButton(new TitleSceneSituation()));
            AddButton("BattleReady", clicked: () => OnClickSceneButton(new BattleReadySceneSituation()));
            AddButton("Battle", clicked: () => OnClickSceneButton(new BattleSceneSituation()));
            yield break;
        }

        private void OnClickSceneButton(SceneSituation sceneSituation)
        {
            var sceneSituationContainer = DebugManager.ServiceContainer.Get<SceneSituationContainer>();
            sceneSituationContainer?.Transition(sceneSituation);
        }
    }
}

#endif // DEVELOPMENT_BUILD || UNITY_EDITOR