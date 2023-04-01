namespace dtank
{
    public interface IDamageReceiver
    {
        string Name { get; }
        bool ReceiveDamage(IAttacker attacker);
    }
}