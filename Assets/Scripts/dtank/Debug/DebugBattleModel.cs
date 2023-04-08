#if DEVELOPMENT_BUILD || UNITY_EDITOR

using System;
using GameFramework.Core;
using UniRx;

namespace dtank
{
    public class BattleDebugModel : IDisposable
    {
        public readonly ReactiveProperty<bool> TimerStopFlag = new();
        public readonly ReactiveProperty<bool> NoReceiveDamageFlag = new();
        public readonly ReactiveProperty<bool> NoDealDamageFlag = new();
        public readonly Subject<Unit> OnDamageMyself = new();
        public readonly Subject<Unit> OnForceTimeUp = new();

        private BattleModel _model;
        private readonly DisposableScope _scope = new();

        public void Setup(BattleModel model)
        {
            _model = model;

            Bind();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        private void Bind()
        {
            OnForceTimeUp
                .TakeUntil(_scope)
                .Subscribe(_ => _model.RuleModel.TimerModel.Update(_model.RuleModel.TimerModel.InitTime))
                .ScopeTo(_scope);

            foreach (var tankModel in _model.TankModels)
            {
                if (tankModel.CharacterType == CharacterType.Player)
                {
                    OnDamageMyself
                        .TakeUntil(_scope)
                        .Subscribe(_ => tankModel.Damage(null))
                        .ScopeTo(_scope);
                }
            }
        }
    }
}

#endif // DEVELOPMENT_BUILD || UNITY_EDITOR