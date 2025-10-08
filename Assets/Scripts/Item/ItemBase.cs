using UnityEngine;

namespace HLO.Item
{
    public abstract class ItemBase : MonoBehaviour
    {
        public abstract string Name { get; }
        public abstract string Description { get; }

        public virtual void Get(GameObject player)
        {
            Use(player);
            Destroy(gameObject);
        }

        public abstract void Use(GameObject player);
    }
}