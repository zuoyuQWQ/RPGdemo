using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
 
public enum ItemType
{
    Material,
    Equipment
}

[CreateAssetMenu(fileName ="新物品数据",menuName ="Data/item")]

public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public Sprite icon;

    [Range(0,100)]
    public float dropChance;


    protected StringBuilder sb = new StringBuilder();

    public virtual string GetDescription()
    {
        return "";
    }
}
