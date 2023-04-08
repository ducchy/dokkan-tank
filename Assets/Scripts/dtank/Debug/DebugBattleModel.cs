#if DEVELOPMENT_BUILD || UNITY_EDITOR

using System;
using GameFramework.Core;
using UniRx;
using UnityDebugSheet.Runtime.Core.Scripts;

namespace dtank
{
    public class BattleDebugModel : IDisposable
    {
        public bool TimerStopFlag;
        public bool NoReceiveDamageFlag;
        public bool NoDealDamageFlag;
        public readonly Subject<Unit> OnDamageMyself = new();
        public readonly Subject<Unit> OnForceTimeUp = new();

        private readonly DisposableScope _scope = new();

        public BattleModel BattleModel { get; private set; }

        public void Setup(BattleModel battleModel)
        {
            BattleModel = battleModel;

            Bind();
        }

        public void Dispose()
        {
            BattleModel = null;

            var currentPage = DebugSheet.Instance.CurrentDebugPage;
            switch (currentPage)
            {
                case DtankBattleTankInfoDebugPage:
                    DebugSheet.Instance.PopPage(true, 2);
                    break;
                case DtankBattleTankInfoListDebugPage:
                    DebugSheet.Instance.PopPage(true);
                    break;
            }

            _scope.Dispose();
        }

        private void Bind()
        {
            OnForceTimeUp
                .TakeUntil(_scope)
                .Subscribe(_ => BattleModel.RuleModel.TimerModel.Update(BattleModel.RuleModel.TimerModel.InitTime))
                .ScopeTo(_scope);

            foreach (var tankModel in BattleModel.TankModels)
            {
                if (tankModel.CharacterType != CharacterType.Player)
                    continue;

                var model = tankModel;
                OnDamageMyself
                    .TakeUntil(_scope)
                    .Subscribe(_ =>
                    {
                        // 被ダメ0判定を一時的に無効化
                        var temp = NoReceiveDamageFlag;
                        NoReceiveDamageFlag = false;
                        model.Damage(null);
                        NoReceiveDamageFlag = temp;
                    })
                    .ScopeTo(_scope);
            }
        }
    }
}

#endif // DEVELOPMENT_BUILD || UNITY_EDITOR