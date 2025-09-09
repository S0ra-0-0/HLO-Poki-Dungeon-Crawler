using UnityEngine;

public class SwordAttack : IAttackType
{
    public float Cooldown => 0.5f;

    public void Attack(Player player)
    {
        Debug.Log("Sword Attack!");
        // Implement sword attack logic here (e.g., melee hitbox, animation)
    }

    public void heavyAttack(Player player)
    {
        Debug.Log("Sword Heavy Attack!");
        // Implement heavy sword attack logic here (e.g., larger hitbox, more damage)
    }   
}