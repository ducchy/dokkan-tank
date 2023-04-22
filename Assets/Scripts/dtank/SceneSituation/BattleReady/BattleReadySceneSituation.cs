using System.Collections;
using GameFramework.Core;
using GameFramework.SituationSystems;
using UnityEngine;

namespace dtank
{
    public class BattleReadySceneSituation : SceneSituation
    {
        protected override string SceneAssetPath => "BattleReady";

        private readonly BattleEntryData _battleEntryData;

        private float _timer;

        public BattleReadySceneSituation()
        {
            _battleEntryData = BattleEntryData.CreateDefaultData();
        }

        public BattleReadySceneSituation(BattleEntryData battleEntryData)
        {
            _battleEntryData = battleEntryData;
        }

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
            ParentContainer.Transition(new BattleSceneSituation(_battleEntryData),
                new CommonFadeTransitionEffect(false, true));
        }
    }
}