using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("�����Ʒ����")]
    [SerializeField] private float chanceToLooseItems;
    [SerializeField] private float chanceToLooseMaterials;

    public override void GenerateDrop()
    {
        Inventory inventory = Inventory.Instance;

        List<InventoryItem> itemToUnequip = new List<InventoryItem>();
        List<InventoryItem> materialToLoose = new List<InventoryItem>();

        #region װ�������߼�
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


        #region ��Ʒ�����߼�
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
