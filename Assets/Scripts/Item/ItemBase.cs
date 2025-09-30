using UnityEngine;

namespace HLO.Item
{
    public abstract class ItemBase : MonoBehaviour
    {
        public abstract string Name { get; }
        public abstract string Description { get; }

        public abstract void Get(GameObject player);
    }
}