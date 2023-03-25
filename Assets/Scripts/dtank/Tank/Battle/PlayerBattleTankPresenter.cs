using GameFramework.Core;
using UniRx;

namespace dtank
{
    public class PlayerBattleTankPresenter : BattleTankPresenterBase
    {
        private readonly BattleTankStatusUiView _statusUiView;

        public PlayerBattleTankPresenter(
            BattleTankController controller,
            BattleModel model,
            BattleTankModel tankModel,
            BattleTankActor actor,
            IBehaviourSelector behaviourSelector,
            BattleTankStatusUiView statusUiView)
            : base(controller, model, tankModel, actor, behaviourSelector)
        {
            _statusUiView = statusUiView;
            
            Bind();
            SetEvents();
        }

        protected override void BindInternal()
        {
            _tankModel.Hp
                .TakeUntil(_scope)
                .Subscribe(_statusUiView.SetHp)
                .ScopeTo(_scope);
        }
    }
}