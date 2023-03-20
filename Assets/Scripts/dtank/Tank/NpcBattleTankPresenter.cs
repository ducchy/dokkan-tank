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
            }
        }

        protected override void OnAnimatorStateExit(BattleTankAnimatorState animState)
        {
            switch (animState)
            {
                case BattleTankAnimatorState.Damage:
                    _behaviourSelector.EndDamage();
                    break;
                case BattleTankAnimatorState.ShotStraight:
                    _behaviourSelector.EndShotStraight();
                    break;
            }
            
            base.OnAnimatorStateExit(animState);
        }

        protected override void OnDead()
        {
            base.OnDead();
            
            _behaviourSelector.SetActive(false);
        }

        public override void OnChangedState(BattleState state)
        {
            base.OnChangedState(state);
            
            _behaviourSelector.SetActive(state == BattleState.Playing);
        }
    }
}