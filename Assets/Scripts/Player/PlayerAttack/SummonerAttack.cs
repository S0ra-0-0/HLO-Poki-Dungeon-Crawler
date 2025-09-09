using UnityEngine;

public class SummonerAttack : IAttackType
{
    public float Cooldown => 1.5f;

    public void Attack(Player player)
    {
        Debug.Log("Summoner Attack!");
        // Summon minion, etc.
    }

    public void heavyAttack(Player player)
    {
        Debug.Log("Summoner Heavy Attack!");
        // Summon stronger minion, etc.
    }
}