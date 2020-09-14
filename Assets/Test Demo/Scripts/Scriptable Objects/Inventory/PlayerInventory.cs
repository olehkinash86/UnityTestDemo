using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Inventory System/Player Inventory")]
public class PlayerInventory : Inventory, ISerializationCallbackReceiver
{
    // Serialized Fields
    [Header("Mount Configuration and Starting Items")]
    [SerializeField] protected List<InventoryWeaponMountInfo>   _weaponMounts   = new List<InventoryWeaponMountInfo>();
    [SerializeField] protected List<InventoryAmmoMountInfo>     _ammoMounts     = new List<InventoryAmmoMountInfo>();
    [SerializeField] protected List<InventoryFoodMountInfo> _foodMounts = new List<InventoryFoodMountInfo>();

    [Header("Shared Variables - Broadcasters")]
    [SerializeField] protected SharedVector3            _playerPosition     = null;
    [SerializeField] protected SharedVector3            _playerDirection    = null;
   
    // Private
    // Runtime Mount Lists
    protected List<InventoryWeaponMountInfo>    _weapons    = new List<InventoryWeaponMountInfo>();
    protected List<InventoryAmmoMountInfo>      _ammo       = new List<InventoryAmmoMountInfo>();
    protected List<InventoryFoodMountInfo>  _foods   = new List<InventoryFoodMountInfo>();

    // ISerializationCallbackReceiver
    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        // Clear our runtime lists
        _weapons.Clear();
        _ammo.Clear();
        _foods.Clear();

        // Clone inspector lists into runtime lists
        foreach (InventoryWeaponMountInfo info in _weaponMounts)
        {
            InventoryWeaponMountInfo clone = new InventoryWeaponMountInfo {Weapon = info.Weapon};
            _weapons.Add(clone);
        }

        foreach (InventoryAmmoMountInfo info in _ammoMounts)
        {
            InventoryAmmoMountInfo clone = new InventoryAmmoMountInfo {Ammo = info.Ammo};
            _ammo.Add(clone);
        }

        foreach (InventoryFoodMountInfo info in _foodMounts)
        {
            InventoryFoodMountInfo clone = new InventoryFoodMountInfo {Item = info.Item};
            _foods.Add(clone);
        }

    }

    public override InventoryWeaponMountInfo GetWeapon(int mountIndex)
    {
        if (mountIndex < 0 || mountIndex >= _weapons.Count) return null;
        return _weapons[mountIndex];
    }

    public override InventoryAmmoMountInfo GetAmmo(int mountIndex)
    {
        if (mountIndex < 0 || mountIndex >= _ammo.Count) return null;
        return _ammo[mountIndex];
    }

    public override InventoryFoodMountInfo GetFood(int mountIndex)
    {
        if (mountIndex < 0 || mountIndex >= _foods.Count) return null;
        return _foods[mountIndex];
    }

    public override CollectableItem DropAmmoItem(int mountIndex)
    {
        if (mountIndex < 0 || mountIndex >= _foods.Count) return null;

        InventoryAmmoMountInfo itemMount = _ammo[mountIndex];
        if (itemMount == null || itemMount.Ammo == null) return null;

        Vector3 position = _playerPosition != null ? _playerPosition.value : Vector3.zero;
        position += _playerDirection != null ? _playerDirection.value : Vector3.zero;

        var droppedItem = itemMount.Ammo.Drop(position);

        OnItemDropped.Invoke(itemMount.Ammo);
        _ammo[mountIndex].Ammo = null;

        return droppedItem;
    }

    public override CollectableItem DropFoodItem(int mountIndex)
    {
        if (mountIndex < 0 || mountIndex >= _foods.Count) return null;

        InventoryFoodMountInfo itemMount = _foods[mountIndex];
        if (itemMount == null || itemMount.Item == null) return null;

        Vector3 position = _playerPosition != null ? _playerPosition.value : Vector3.zero;
        position += _playerDirection != null ? _playerDirection.value : Vector3.zero;

        var droppedItem = itemMount.Item.Drop(position);

        OnItemDropped.Invoke(itemMount.Item);
        _foods[mountIndex].Item = null;

        return droppedItem;
    }

    public override CollectableItem DropWeaponItem(int mountIndex)
    {
        if (mountIndex < 0 || mountIndex >= _weapons.Count) return null;

        InventoryWeaponMountInfo itemMount = _weapons[mountIndex];
        if (itemMount == null || itemMount.Weapon == null) return null;

        InventoryItemWeapon weapon = itemMount.Weapon;

        Vector3 position = _playerPosition != null ? _playerPosition.value : Vector3.zero;
        position += _playerDirection != null ? _playerDirection.value : Vector3.zero;

        var droppedItem = weapon.Drop(position);

        OnItemDropped.Invoke(itemMount.Weapon);
        _weapons[mountIndex].Weapon = null;

        return droppedItem;
    }

    public override bool AddItem(CollectableItem collectableItem)
    {
        if (collectableItem == null || collectableItem.inventoryItem == null) return false;

        InventoryItem invItem = collectableItem.inventoryItem;
        invItem.CollectableItem = collectableItem;

        switch (invItem.Category)
        {
            case InventoryItemType.Consumable:
                AddFoodItem(invItem as InventoryItemFood, collectableItem);
                break;

            case InventoryItemType.Weapon:
                AddWeaponItem(invItem as InventoryItemWeapon, collectableItem as CollectableWeapon);
                break;

            case InventoryItemType.Ammunition:
                AddAmmoItem(invItem as InventoryItemAmmo, collectableItem);
                break;
        }

        OnItemAdded.Invoke(invItem);

        return false;
    }

    protected bool AddFoodItem(InventoryItemFood inventoryItem, CollectableItem collectableItem)
    {
        foreach (var food in _foods)
        {
            if (food.Item == null)
            {
                food.Item = inventoryItem;
                food.attachedItem = collectableItem;
                inventoryItem.Pickup(collectableItem.transform.position);
                return true;
            }
        }

        return false;
    }

    protected bool AddAmmoItem(InventoryItemAmmo inventoryItemAmmo, CollectableItem collectableAmmo)
    {
        foreach (var a in _ammo)
        {
            if (a.Ammo == null)
            {
                a.Ammo = inventoryItemAmmo;
                inventoryItemAmmo.Pickup(collectableAmmo.transform.position);
                return true;
            }
        }

        return false;
    }

    protected bool AddWeaponItem(InventoryItemWeapon inventoryItemWeapon, CollectableWeapon collectableWeapon)
    {
        foreach (var w in _weapons)
        {
            if (w.Weapon == null)
            {
                w.Weapon = inventoryItemWeapon;
                w.Rounds = collectableWeapon.Rounds;
                inventoryItemWeapon.Pickup(collectableWeapon.transform.position);
                return true;
            }
        }

        return false;
    }
}
