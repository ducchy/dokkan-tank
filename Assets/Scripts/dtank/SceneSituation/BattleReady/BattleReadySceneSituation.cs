using System.Collections;
using GameFramework.Core;
using GameFramework.SituationSystems;
using UnityEngine;

namespace dtank
{
    public class BattleReadySceneSituation : SceneSituation
    {
        protected override string SceneAssetPath => "BattleReady";

        private float _timer;

        protected override IEnumerator SetupRoutineInternal(TransitionHandle handle, IScope scope)
        {
            _timer = 0.5f;
            yield break;
        }

        protected override void UpdateInternal()
        {
            if (_timer <= 0f)
                return;
            
            _timer -= Time.deltaTime;
            if (_timer > 0f)
                return;

            TransitionToBattle();
        }

        private void TransitionToBattle()
        {
            ParentContainer.Transition(new BattleSceneSituation(), new CommonFadeTransitionEffect(0f, 0.5f));
        }
    }
}