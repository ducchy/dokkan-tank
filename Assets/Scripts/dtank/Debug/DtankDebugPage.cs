#if DEVELOPMENT_BUILD || UNITY_EDITOR

using System.Collections;
using UnityDebugSheet.Runtime.Core.Scripts;

namespace dtank
{
    public sealed class DtankDebugPage : DefaultDebugPageBase
    {
        protected override string Title => "dtank Debug";

        public override IEnumerator Initialize()
        {
            AddPageLinkButton<DtankSceneDebugPage>("Scene");
            AddPageLinkButton<DtankBattleDebugPage>("Battle");

            yield break;
        }
    }
}

#endif // DEVELOPMENT_BUILD || UNITY_EDITOR