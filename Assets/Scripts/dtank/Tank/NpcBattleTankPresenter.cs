namespace dtank
{
    public class NpcBattleTankPresenter : BattleTankPresenterBase
    {
        public NpcBattleTankPresenter(
            BattleTankController controller,
            BattleModel model,
            BattleTankModel tankModel,
            BattleTankActor actor,
            IBehaviourSelector behaviourSelector)
            : base(controller, model, tankModel, actor, behaviourSelector)
        {
            Bind();
            SetEvents();
        }
    }
}