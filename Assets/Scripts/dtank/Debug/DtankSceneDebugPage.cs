#if DEVELOPMENT_BUILD || UNITY_EDITOR

using System.Collections;
using GameFramework.SituationSystems;
using UnityDebugSheet.Runtime.Core.Scripts;

namespace dtank
{
    public sealed class DtankSceneDebugPage : DefaultDebugPageBase
    {
        protected override string Title => "dtank > Scene";

        public override IEnumerator Initialize()
        {
            AddButton("Title", clicked: () => OnClickSceneButton(new TitleSceneSituation(), true));
            AddButton("BattleReady", clicked: () => OnClickSceneButton(new BattleReadySceneSituation(), false));
            AddButton("Battle", clicked: () => OnClickSceneButton(new BattleSceneSituation(), true));
            yield break;
        }

        private void OnClickSceneButton(SceneSituation sceneSituation, bool withFadeIn)
        {
            var sceneSituationContainer = DebugManager.ServiceContainer.Get<SceneSituationContainer>();
            if (withFadeIn)
                sceneSituationContainer?.Transition(sceneSituation, new CommonFadeTransitionEffect(false, true));
            else
                sceneSituationContainer?.Transition(sceneSituation);
        }
    }
}

#endif // DEVELOPMENT_BUILD || UNITY_EDITOR