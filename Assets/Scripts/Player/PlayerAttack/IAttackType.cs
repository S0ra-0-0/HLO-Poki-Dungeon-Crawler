public interface IAttackType
{
    void Attack(Player player);
    float Cooldown { get; }
}