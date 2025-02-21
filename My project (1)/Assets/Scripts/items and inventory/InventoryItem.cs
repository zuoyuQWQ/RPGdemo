using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public ItemData data;
    public int stackSize =1;

    public InventoryItem(ItemData _newItemData)
    {
        data = _newItemData;
    }

    public void AddStack() => stackSize++;

    public void RemoveStack() => stackSize--;
}
