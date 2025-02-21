using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipmentSlot : UI_ItemSlot
{
    public EquipmentType slotType;

    private void OnValidate()
    {
        gameObject.name = "×°±¸²Û"+ slotType.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        Inventory.Instance.UnequipItem(item.data as ItemDataEquipment);
        Inventory.Instance.AddItem(item.data as ItemDataEquipment);
        CleanUpSlot();

    }
}
