using GameFramework.SituationSystems;

namespace dtank
{
    /// <summary>
    /// MainSystem開始用クラス
    /// </summary>
    public abstract class MainSystemStarterBase : GameFramework.Core.MainSystemStarter
    {
        public override object[] GetArguments()
        {
            return new object[]
            {
                GetStartSituation()
            };
        }

        /// <summary>
        /// 開始シチュエーションの取得
        /// </summary>
        protected abstract SceneSituation GetStartSituation();
    }
}