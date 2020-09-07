using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Inventory System/Items/Weapon")]
public class InventoryItemWeapon : InventoryItem
{
    [SerializeField] protected int _ammoCapacity = 0;
}
