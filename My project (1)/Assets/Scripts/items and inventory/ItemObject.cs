using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ItemData itemData;


    private void SetupVisuals()
    {
        if (itemData == null)
        {
            Debug.Log("��ƷΪ��");
            return;
        }
        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "��Ʒ" + itemData.itemName;
    }

    public void SetupItem(ItemData _itemData, Vector2 _velocity)
    {
        itemData = _itemData;
        rb.velocity = _velocity;
        SetupVisuals();
    }

    //ʰȡ��Ʒ����

    public void PickupItem()
    {
        if (!Inventory.Instance.CanAddItem() && itemData.itemType == ItemType.Equipment)
        {
            rb.velocity = new Vector2(0,5);
            return;
        }
        Inventory.Instance.AddItem(itemData);
        Destroy(gameObject);
    }
}
