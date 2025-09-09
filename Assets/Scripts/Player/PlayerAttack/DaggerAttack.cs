using UnityEngine;

public class DaggerAttack : IAttackType
{
    public float Cooldown => 0.2f;

    public void Attack(Player player)
    {
        Debug.Log("Dagger Combo Attack!");
        // Implement combo logic here
    }
}