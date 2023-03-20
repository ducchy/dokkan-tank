using GameFramework.SituationSystems;

namespace dtank
{
    public class TitleMainSystemStarter : MainSystemStarterBase
    {
        protected override SceneSituation GetStartSituation()
        {
            return new TitleSceneSituation();
        }
    }
}