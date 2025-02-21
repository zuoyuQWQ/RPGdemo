using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ItemTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemDescription;

    void Start()
    {
        
    }

    public void ShowToolTip(ItemDataEquipment _item)
    {
        if(_item == null)
            return;
        itemNameText.text = _item.itemName;
        itemTypeText.text = _item.equpmentType.ToString();
        itemDescription.text = _item.GetDescription();

        if (itemNameText.text.Length > 10)
            itemNameText.fontSize = itemNameText.fontSize * .9f;
        else
            itemNameText.fontSize = 32;

        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        if (itemNameText.text.Length > 10)
            itemNameText.fontSize = 32;
        gameObject.SetActive(false);
    }
}
