using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : InteractiveItem
{
    [SerializeField] protected Inventory _inventory = null;

    [Header("Collectable Item Properties")]
    [SerializeField]
    protected InventoryItem _inventoryItem = null;

    public Inventory inventory => _inventory;
    public InventoryItem inventoryItem => _inventoryItem;
    public bool InBackpack = false;

    public override void Drag(CharacterManager characterManager)
    {
        if (!InBackpack)
        {
            _rigidBody.useGravity = false;
            _rigidBody.isKinematic = true;
            characterManager.SelectedDraggableItem = this;
            transform.position = Camera.main.transform.forward * 1f + Camera.main.transform.position;
            InBackpack = false;
        }
    }

    public override void Drop()
    {
        _rigidBody.useGravity = true;
        _rigidBody.isKinematic = false;
        InBackpack = false;
    }

    public override void Attach()
    {
        _rigidBody.useGravity = false;
        _rigidBody.isKinematic = true;
        InBackpack = true;
    }
}
