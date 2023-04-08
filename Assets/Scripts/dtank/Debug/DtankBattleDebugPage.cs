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
            AddButton("自傷ダメージ", clicked: () => Model.OnDamageMyself.OnNext(Unit.Default));
            AddButton("強制タイムアップ", clicked: () => Model.OnForceTimeUp.OnNext(Unit.Default));
            AddSwitch(Model.TimerStopFlag, "タイマー停止",
                valueChanged: value => Model.TimerStopFlag = value);
            AddSwitch(Model.NoReceiveDamageFlag, "被ダメージ0",
                valueChanged: value => Model.NoReceiveDamageFlag = value);
            AddSwitch(Model.NoDealDamageFlag, "与ダメージ0",
                valueChanged: value => Model.NoDealDamageFlag = value);
            AddPageLinkButton<DtankBattleTankInfoListDebugPage>("Tank Info");
            yield break;
        }
    }
}

#endif // DEVELOPMENT_BUILD || UNITY_EDITOR