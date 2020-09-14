using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public abstract class InventoryMountInfo
{
    public CollectableItem attachedItem = null;
}

[System.Serializable]
public class InventoryWeaponMountInfo : InventoryMountInfo
{
    public InventoryItemWeapon Weapon = null;
    public int Rounds = 0;
}

[System.Serializable]
public class InventoryAmmoMountInfo : InventoryMountInfo
{
    public InventoryItemAmmo Ammo = null;
}

[System.Serializable]
public class InventoryFoodMountInfo : InventoryMountInfo
{
    public InventoryItemFood Item = null;
}

[System.Serializable]
public class InventoryItemAddEvent : UnityEvent<InventoryItem> { }
[System.Serializable]
public class InventoryItemDropEvent : UnityEvent<InventoryItem> { }

public abstract class Inventory : ScriptableObject
{
    public InventoryItemAddEvent OnItemAdded = new InventoryItemAddEvent();
    public InventoryItemDropEvent OnItemDropped = new InventoryItemDropEvent();

    public abstract InventoryWeaponMountInfo GetWeapon(int mountIndex);
    public abstract InventoryAmmoMountInfo GetAmmo(int mountIndex);
    public abstract InventoryFoodMountInfo GetFood(int mountIndex);

    public abstract CollectableItem DropAmmoItem(int mountIndex);
    public abstract CollectableItem DropFoodItem(int mountIndex);
    public abstract CollectableItem DropWeaponItem(int mountIndex);

    public abstract bool AddItem(CollectableItem collectableItem);
}
