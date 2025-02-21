using Autodesk.Fbx;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public List<ItemData> startingItems;

    public List<InventoryItem> equipment;
    public Dictionary<ItemDataEquipment, InventoryItem> equipmentDictiatiory;

    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictiatiory;

    public List<InventoryItem> stash;
    public Dictionary<ItemData,InventoryItem> stashDictiatiory;

    [Header("库存UI")]

    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentSlotParent;
    [SerializeField] private Transform statSlotParent;

    private UI_ItemSlot[] inventoryItemSlot;
    private UI_ItemSlot[] stashItemSLot;
    private UI_EquipmentSlot[] equipmentSlot;
    private UI_StatSlot[] statSlot;

    [Header("物品cd")]
    [SerializeField] private float lastTimeUseFlask;
    [SerializeField] private float lastTimeUseAmor;

    private float flaskCooldown;
    private float amorCooldown;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Start()
    {


        equipment = new List<InventoryItem>();
        equipmentDictiatiory = new Dictionary<ItemDataEquipment, InventoryItem>();

        inventory = new List<InventoryItem>();
        inventoryDictiatiory = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictiatiory = new Dictionary<ItemData, InventoryItem>();

        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stashItemSLot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equipmentSlot = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();

        statSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();

        AddStartingItems();
    }

    private void AddStartingItems()
    {
        for (int i = 0; i < startingItems.Count; i++)
        {
            AddItem(startingItems[i]);
        }
    }

    //装备的基本逻辑方法

    public void EquipItem(ItemData _item)
    {
        ItemDataEquipment newEqupment = _item as ItemDataEquipment;
        InventoryItem newItem = new InventoryItem(newEqupment);

        ItemDataEquipment oldEquipment = null;

        foreach (KeyValuePair<ItemDataEquipment, InventoryItem> item in equipmentDictiatiory)
        {
            if (item.Key.equpmentType == newEqupment.equpmentType)
                oldEquipment = item.Key;
        }
        if(oldEquipment != null)
        {
            UnequipItem(oldEquipment);
        }


        equipment.Add(newItem);
        equipmentDictiatiory.Add(newEqupment, newItem);

        newEqupment.AddModfiers();

        RemoveItem(_item);

    }

    //提供了的脱下装备方法

    public void UnequipItem(ItemDataEquipment _itemToRemove)
    {
        if(_itemToRemove == null)
            return;
        if (equipmentDictiatiory.TryGetValue(_itemToRemove, out InventoryItem value))
        {
            AddItem(_itemToRemove);
            equipment.Remove(value);
            equipmentDictiatiory.Remove(_itemToRemove);
            _itemToRemove.RemoveModfiers();
        }
    }

    private void UpdateSlotUI()
    {
        for(int i =0; i < equipmentSlot.Length; i++)
        {
            foreach (KeyValuePair<ItemDataEquipment, InventoryItem> item in equipmentDictiatiory)
            {
                if (item.Key.equpmentType == equipmentSlot[i].slotType)
                    equipmentSlot[i].UpdateSlot(item.Value);
                    
            }
        }


        for(int i = 0; i < inventoryItemSlot.Length; i++)
        {
            inventoryItemSlot[i].CleanUpSlot();

        }
        for(int i = 0; i < stashItemSLot.Length; i++)
        {
            stashItemSLot[i].CleanUpSlot();
        }
        for(int i = 0; i < inventory.Count; i++)
        {
            inventoryItemSlot[i].UpdateSlot(inventory[i]);
        }

        for(int i = 0; i < stash.Count; i++)
        {
            stashItemSLot[i].UpdateSlot(stash[i]);
        }

        for(int i =0; i < statSlot.Length; i++)//更新角色属性的UI
        {
            statSlot[i].UpdateStatValueUI();
        }
    }

    public void AddItem(ItemData _item)
    {
        if (_item.itemType == ItemType.Equipment && CanAddItem())
        {
            AddToInventory(_item);
        }
        else if(_item.itemType == ItemType.Material)
        {
            AddToStash(_item);
        }
        UpdateSlotUI();
    }

    private void AddToStash(ItemData _item)
    {
        if (stashDictiatiory.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictiatiory.Add(_item, newItem);
        }
    }

    private void AddToInventory(ItemData _item)
    {
        if (inventoryDictiatiory.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictiatiory.Add(_item, newItem);
        }
    }

    public void RemoveItem(ItemData _item)
    {
        if(inventoryDictiatiory.TryGetValue(_item, out InventoryItem value))
        {
            if(value.stackSize <= 1)
            {
                inventory.Remove(value);
                inventoryDictiatiory.Remove(_item);
            }
            else
                value.RemoveStack();
        }

        if(stashDictiatiory.TryGetValue(_item, out InventoryItem stashValue))
        {
            if(stashValue.stackSize <= 1)
            {
                stash.Remove(stashValue); 
                stashDictiatiory.Remove(_item);
            }
            else
                stashValue.RemoveStack();
        }
        UpdateSlotUI();
    }

    public bool CanAddItem()
    {
        if(inventory.Count >= inventoryItemSlot.Length)
        {
            Debug.Log("物品溢出");
            return false;
        }
        return true;
    }

    public bool CanCraft(ItemDataEquipment _itemToCraft,List<InventoryItem> _requiredMaterials)
    {
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();

        for(int i =0; i < _requiredMaterials.Count; i++)
        {
            if (stashDictiatiory.TryGetValue(_requiredMaterials[i].data,out InventoryItem stashValue))
            {
                Debug.Log($"检查材料: {stashValue.data.name}, 存量: {stashValue.stackSize}, 所需: {_requiredMaterials[i].stackSize}");
                if (stashValue.stackSize < _requiredMaterials[i].stackSize)
                {
                    Debug.Log("材料不足1");
                    return false;
                }
                else
                {
                    materialsToRemove.Add(stashValue);
                }
            }
            else
            {
                Debug.Log("材料不足");
                return false;
            }
        }

        for(int i = 0; i < materialsToRemove.Count; i++)
        {
            RemoveItem(materialsToRemove[i].data);
        }
        AddItem(_itemToCraft);
        Debug.Log("你的东西" + _itemToCraft.name);

        return true;
    }

    public List<InventoryItem> GetEquipmentList() => equipment;

    public List<InventoryItem> GetStashList() => stash;

    public ItemDataEquipment GetEquipment(EquipmentType _type)
    {
        ItemDataEquipment equipedItem = null;

        foreach (KeyValuePair<ItemDataEquipment, InventoryItem> item in equipmentDictiatiory)
        {
            if (item.Key.equpmentType == _type)
                equipedItem = item.Key;
        }

        return equipedItem;
    }

    public void UseFlask()
    {
        ItemDataEquipment currentFlask = GetEquipment(EquipmentType.Flask);

        if (currentFlask == null)
            return;

        bool canUseFlask = Time.time > lastTimeUseFlask + flaskCooldown;

        if (canUseFlask)
        {
            flaskCooldown = currentFlask.itemCooldown;
            currentFlask.Effect(null);
            lastTimeUseFlask = Time.time;
        }
        else
        {
            Debug.Log("治疗瓶冷却ing");
        }
    }

    public bool CanUseAmor()
    {
        ItemDataEquipment currentAmor = GetEquipment(EquipmentType.Armor);

        if (Time.time > lastTimeUseAmor + amorCooldown)
        {
            amorCooldown = currentAmor.itemCooldown;
            lastTimeUseAmor = Time.time;
            return true;
        }
        Debug.Log("护甲能力冷却ing"); 
        return false;
    }

}
