public interface IAttackType
{
    void Attack(Player player);
    void heavyAttack(Player player);    
    float Cooldown { get; }
}