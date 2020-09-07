using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class Backpack : CollectableItem
{
	[SerializeField] private GameObject _inventoryUI = null;

	private PlayerInventoryUI _playerInventoryUI = null;

	protected override void Start()
	{
		_playerInventoryUI = _inventoryUI.GetComponent<PlayerInventoryUI>();
		base.Start();
	}

	public override void Activate(CharacterManager characterManager)
    {
		if (_inventoryUI == null || _inventory == null)
			return;

		if (characterManager.SelectedDraggableItem != null)
		{
			_inventory.AddItem(characterManager.SelectedDraggableItem);
			Destroy(characterManager.SelectedDraggableItem.gameObject);
		}

		_inventoryUI.SetActive(true);
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		characterManager.SelectedStorageItem = this;
	}

	public override void Deactivate(CharacterManager characterManager)
    {
        switch (_playerInventoryUI.selectedPanelType)
        {
			case InventoryPanelType.Weapons:
				_inventory.DropWeaponItem(0);
				break;
			case InventoryPanelType.AmmoBelt:
				_inventory.DropAmmoItem(0);
				break;
			case InventoryPanelType.Backpack:
				_inventory.DropFoodItem(0);
				break;
		}

		_inventoryUI.SetActive(false);
		characterManager.SelectedStorageItem = null;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
}

