using UnityEngine;

namespace HLO.Item
{
    public interface IItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public void Get(GameObject player);
    }
}