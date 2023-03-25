namespace dtank
{
    public interface IDamageReceiver
    {
        bool ReceiveDamage(IAttacker attacker);
    }
}