// Unity
using UnityEngine;

namespace HLO.Item
{
    public class HealItem : ItemBase
    {
        public override string Name => "Heal";
        public override string Description => $"Restores {healAmount} HP";

        [SerializeField] private int healAmount = 1;

        public override void Use(GameObject player)
        {
            player.GetComponent<PlayerHealth>().HealHP(healAmount);
        }
    }
}