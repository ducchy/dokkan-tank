namespace dtank
{
    public class NpcBattleTankPresenter : BattleTankPresenterBase
    {
        private readonly NpcBehaviourSelector _behaviourSelector;
        
        public NpcBattleTankPresenter(
            BattleTankController controller,
            BattleTankModel model,
            BattleTankActor actor,
            NpcBehaviourSelector behaviourSelector)
            : base(controller, model, actor, behaviourSelector)
        {
            _behaviourSelector = behaviourSelector;
        }

        public void Update(float deltaTime)
        {
            _behaviourSelector.Update(deltaTime);
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
            base.OnAnimatorStateExit(animState);

            switch (animState)
            {
                case BattleTankAnimatorState.Damage:
                    _behaviourSelector.EndDamage();
                    break;
                case BattleTankAnimatorState.ShotStraight:
                    _behaviourSelector.EndShotStraight();
                    break;
            }
        }

        protected override void OnDead()
        {
            base.OnDead();
            
            _behaviourSelector.SetActive(false);
        }

        public override void OnChangedState(BattleState prev, BattleState current)
        {
            base.OnChangedState(prev, current);
            
            _behaviourSelector.SetActive(current == BattleState.Playing);
        }
    }
}