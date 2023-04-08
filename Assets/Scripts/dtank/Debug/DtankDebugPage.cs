#if DEVELOPMENT_BUILD || UNITY_EDITOR

using System.Collections;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;

namespace dtank
{
    public sealed class DtankDebugPage : DefaultDebugPageBase
    {
        protected override string Title => "dtank Debug Page";

        public override IEnumerator Initialize()
        {
            AddButton("Example Button", clicked: () => { Debug.Log("Clicked"); });
            yield break;
        }
    }
}

#endif // DEVELOPMENT_BUILD || UNITY_EDITOR