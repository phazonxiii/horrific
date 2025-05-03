using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace HorrorEngine
{
    [Serializable]
    public class ShopContentValueEntry
    {
        [FormerlySerializedAs("Value")]
        public GameAttribute Attribute;
        public int Amount;
    }

    [Serializable]
    public class ShopContentEntry
    {
        public InventoryEntry Item;
        public ShopContentValueEntry[] Cost;
    }

    [Serializable]
    public class ShopContentCategory
    {
        public LocalizableText Name;
        public LocalizableSprite Icon;
        public List<ShopContentEntry> Items;
    }

    [CreateAssetMenu(menuName = "Horror Engine/Shop/Content")]
    public class ShopContent : ScriptableObject
    {
        [FormerlySerializedAs("AvailableItems")]
        public List<ShopContentCategory> ProductSelling;
    }
}