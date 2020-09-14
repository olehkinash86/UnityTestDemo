using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Backpack : CollectableItem
{
	[SerializeField] private GameObject _inventoryUI = null;

	private PlayerInventoryUI _playerInventoryUI = null;

	protected override void Start()
	{
		_playerInventoryUI = _inventoryUI.GetComponent<PlayerInventoryUI>();
		base.Start();
	}

	public override void Drag(CharacterManager characterManager)
    {
		if (_inventoryUI == null || _inventory == null)
			return;

		if (characterManager.SelectedDraggableItem != null)
		{
			_inventory.AddItem(characterManager.SelectedDraggableItem);
            var position = characterManager.SelectedDraggableItem.GetAttachPosition();
            var rotation = characterManager.SelectedDraggableItem.GetAttachRotation();

            characterManager.SelectedDraggableItem.transform.parent = transform;

			characterManager.SelectedDraggableItem.transform.localPosition = position;
            characterManager.SelectedDraggableItem.transform.localRotation = rotation;

			characterManager.SelectedDraggableItem.Attach();
			characterManager.SelectedDraggableItem = null;
		}

		_inventoryUI.SetActive(true);
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		characterManager.SelectedStorageItem = this;
	}

	public override void Drop()
    {
		CollectableItem droppedItem = null;

        switch (_playerInventoryUI.selectedPanelType)
        {
			case InventoryPanelType.Weapons:
                droppedItem = _inventory.DropWeaponItem(0);
				break;
			case InventoryPanelType.AmmoBelt:
                droppedItem = _inventory.DropAmmoItem(0);
				break;
			case InventoryPanelType.Backpack:
                droppedItem = _inventory.DropFoodItem(0);
				break;
		}

		if(droppedItem != null)
            droppedItem.transform.parent = null;

		_inventoryUI.SetActive(false);
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
}

