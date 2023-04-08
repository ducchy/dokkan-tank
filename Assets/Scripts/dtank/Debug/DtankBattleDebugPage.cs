#if DEVELOPMENT_BUILD || UNITY_EDITOR

using System;
using System.Collections;
using UniRx;
using UnityDebugSheet.Runtime.Core.Scripts;

namespace dtank
{
    public class DtankBattleDebugPageModel : IDisposable
    {
        public readonly ReactiveProperty<bool> TimerStopFlag = new();
        
        public Action OnDamageMyself;
        public Action OnForceTimeUp;

        public void Dispose()
        {
            OnDamageMyself = null;
            OnForceTimeUp = null;

            TimerStopFlag.Dispose();
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
            AddButton("強制タイムアップ", clicked: Model?.OnForceTimeUp);
            AddSwitch(Model != null && Model.TimerStopFlag.Value, "タイマー停止", valueChanged: value =>
            {
                if (Model != null)
                    Model.TimerStopFlag.Value = value;
            });
            yield break;
        }
    }
}

#endif // DEVELOPMENT_BUILD || UNITY_EDITOR