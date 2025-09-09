using UnityEngine;

public class SummonerAttack : IAttackType
{
    public float Cooldown => 1.5f;

    public void Attack(Player player)
    {
        Debug.Log("Summoner Attack!");
        // Summon minion, etc.
    }
}