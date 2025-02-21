using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CraftSlot : UI_ItemSlot
{
    private void OnEnable()
    {
        UpdateSlot(item);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        ItemDataEquipment craftData = item.data as ItemDataEquipment;

        Inventory.Instance.CanCraft(craftData, craftData.craftingMaterials);
    }
}
