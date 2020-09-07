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

    public override void Activate(CharacterManager characterManager)
    {
        _rigidBody.useGravity = false;
        characterManager.SelectedDraggableItem = this;
        transform.position = Camera.main.transform.forward * 1 + Camera.main.transform.position;
    }

    public override void Deactivate(CharacterManager characterManager)
    {
        _rigidBody.useGravity = true;
        characterManager.SelectedDraggableItem = null;
    }
}
