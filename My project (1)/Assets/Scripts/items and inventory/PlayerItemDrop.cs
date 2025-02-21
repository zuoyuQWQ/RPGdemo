using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("玩家物品掉落")]
    [SerializeField] private float chanceToLooseItems;
    [SerializeField] private float chanceToLooseMaterials;

    public override void GenerateDrop()
    {
        Inventory inventory = Inventory.Instance;

        List<InventoryItem> itemToUnequip = new List<InventoryItem>();
        List<InventoryItem> materialToLoose = new List<InventoryItem>();

        #region 装备掉落逻辑
        foreach (InventoryItem item in inventory.GetEquipmentList())
        {
            if(Random.Range(0,100) <= chanceToLooseItems)
            {
                DropItem(item.data);
                itemToUnequip.Add(item);
            }
        }
        for(int i = 0; i < itemToUnequip.Count; i++)
        {
            inventory.UnequipItem(itemToUnequip[i].data as ItemDataEquipment);
        }
        #endregion


        #region 物品掉落逻辑
        foreach (InventoryItem item in inventory.GetStashList())
        {
            if(Random.Range(0,100)<= chanceToLooseMaterials)
            {
                DropItem(item.data);
                materialToLoose.Add(item);
            }
        }
        for(int i = 0;i < materialToLoose.Count; i++)
        {
            inventory.RemoveItem(materialToLoose[i].data);
        }
        #endregion

    }
}
