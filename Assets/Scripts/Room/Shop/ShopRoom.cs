// System
using System;
using System.Collections.Generic;
using System.Linq;

// Unity
using UnityEngine;

// HLO
using HLO.Item;

namespace HLO.Room
{
    public class ShopRoom : RoomBase
    {
        [SerializeField] private ShopItemScriptableObjectScript shopItemScriptableObject;
        [SerializeField] private Transform[] displayingPlaces;

        protected void Start()
        {
            SetRoomType(RoomType.Shop);
            RegisterOnEnterRoom(ClearRoom);
            RegisterOnEnterRoom(Display);
            RegisterOnClearRoom(() =>
            {
                UnregisterOnEnterRoom(ClearRoom);
                UnregisterOnEnterRoom(Display);
            });
        }

        protected void Display()
        {
            List<ShopItem> shopItemList = shopItemScriptableObject.shopItems.ToList();

            for (int i = 0; i < displayingPlaces.Length; i++)
            {
                if (shopItemList.Count == 0) break;

                ShopItem item = shopItemList[UnityEngine.Random.Range(0, shopItemList.Count)];
                Instantiate(item.prefab, displayingPlaces[i].position, Quaternion.identity).
                    GetComponent<DisplayedItem>().SetItem(item.name, item.description, item.price);

                shopItemList.Remove(item);
            }
        }
    }
}