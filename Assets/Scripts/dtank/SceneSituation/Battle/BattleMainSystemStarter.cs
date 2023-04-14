using GameFramework.SituationSystems;
using UnityEngine;

namespace dtank
{
    public class BattleMainSystemStarter : MainSystemStarterBase
    {
        [SerializeField] private BattleEntryData _battleEntryData;
        
        protected override SceneSituation GetStartSituation()
        {
            return new BattleSceneSituation(_battleEntryData);
        }
    }
}