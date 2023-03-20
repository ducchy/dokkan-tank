using GameFramework.SituationSystems;

namespace dtank
{
    public class BattleMainSystemStarter : MainSystemStarterBase
    {
        protected override SceneSituation GetStartSituation()
        {
            return new BattleSceneSituation();
        }
    }
}