namespace dtank
{
    public class NpcBattleTankPresenter : BattleTankPresenterBase
    {
        private readonly IBehaviourSelector _behaviourSelector;
        
        public NpcBattleTankPresenter(
            BattleTankController controller,
            BattleTankModel model,
            BattleTankActor actor,
            IBehaviourSelector behaviourSelector)
            : base(controller, model, actor, behaviourSelector)
        {
            _behaviourSelector = behaviourSelector;
            
            Bind();
            SetEvents();
        }

        protected override void OnStateChanged(BattleTankState state)
        {
            base.OnStateChanged(state);

            switch (state)
            {
                case BattleTankState.Damage:
                    _behaviourSelector.BeginDamage();
                    break;
                case BattleTankState.Dead:
                    _behaviourSelector.SetActive(false);
                    break;
            }
        }

        public override void OnChangedState(BattleState state)
        {
            base.OnChangedState(state);
            
            _behaviourSelector.SetActive(state == BattleState.Playing);
        }
    }
}