using System;
using UnityEngine;

[Serializable]
public struct ShopItem 
{
    public string name;
    [TextArea] public string description;
    public int price;

    public GameObject prefab;
}
