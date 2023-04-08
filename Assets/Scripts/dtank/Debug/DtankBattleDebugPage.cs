#if DEVELOPMENT_BUILD || UNITY_EDITOR

using System.Collections;
using JetBrains.Annotations;
using UniRx;
using UnityDebugSheet.Runtime.Core.Scripts;

namespace dtank
{
    public sealed class DtankBattleDebugPage : DefaultDebugPageBase
    {
        protected override string Title => "dtank > Battle";

        [NotNull] private BattleDebugModel Model => DebugManager.BattleDebugModel;

        public override IEnumerator Initialize()
        {
            DebugManager.ServiceContainer.Set(this);

            AddButton("自傷ダメージ", clicked: () => Model.OnDamageMyself.OnNext(Unit.Default));
            AddButton("強制タイムアップ", clicked: () => Model.OnForceTimeUp.OnNext(Unit.Default));
            AddSwitch(Model.TimerStopFlag.Value, "タイマー停止",
                valueChanged: value => Model.TimerStopFlag.Value = value);
            AddSwitch(Model.NoReceiveDamageFlag.Value, "被ダメージ0",
                valueChanged: value => Model.NoReceiveDamageFlag.Value = value);
            AddSwitch(Model.NoDealDamageFlag.Value, "与ダメージ0",
                valueChanged: value => Model.NoDealDamageFlag.Value = value);
            yield break;
        }
    }
}

#endif // DEVELOPMENT_BUILD || UNITY_EDITOR