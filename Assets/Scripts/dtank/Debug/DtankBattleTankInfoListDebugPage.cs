#if DEVELOPMENT_BUILD || UNITY_EDITOR

using System.Collections;
using JetBrains.Annotations;
using UnityDebugSheet.Runtime.Core.Scripts;

namespace dtank
{
    public sealed class DtankBattleTankInfoListDebugPage : DefaultDebugPageBase
    {
        protected override string Title => "Battle > Tank Info";

        [NotNull] private BattleDebugModel Model => DebugManager.BattleDebugModel;

        public override IEnumerator Initialize()
        {
            if (Model.BattleModel == null)
            {
                AddLabel("Battle中のみ表示");
                yield break;
            }

            var tankModels = Model.BattleModel.TankModels;
            if (tankModels.Count == 0)
                yield break;

            foreach (var tankModel in tankModels)
            {
                AddPageLinkButton<DtankBattleTankInfoDebugPage>(tankModel.Name,
                    onLoad: tuple => tuple.page.SetModel(tankModel),
                    textColor: tankModel.ActorModel.SetupData.Color);
            }
        }
    }
}

#endif // DEVELOPMENT_BUILD || UNITY_EDITOR