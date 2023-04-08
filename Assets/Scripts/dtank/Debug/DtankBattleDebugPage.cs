#if DEVELOPMENT_BUILD || UNITY_EDITOR

using System;
using System.Collections;
using UnityDebugSheet.Runtime.Core.Scripts;

namespace dtank
{
    public class DtankBattleDebugPageModel : IDisposable
    {
        public Action OnDamageMyself;

        public void Dispose()
        {
            OnDamageMyself = null;
        }
    }
    
    public sealed class DtankBattleDebugPage : DefaultDebugPageBase
    {
        protected override string Title => "dtank Debug > Battle";

        private DtankBattleDebugPageModel Model => DebugManager.ServiceContainer.Get<DtankBattleDebugPageModel>();

        public override IEnumerator Initialize()
        {
            DebugManager.ServiceContainer.Set(this);
            
            AddButton("自傷ダメージ", clicked: Model?.OnDamageMyself);
            yield break;
        }
    }
}

#endif // DEVELOPMENT_BUILD || UNITY_EDITOR