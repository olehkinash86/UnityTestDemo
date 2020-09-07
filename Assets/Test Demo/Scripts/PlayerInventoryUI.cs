using UnityEngine;
using UnityEngine.UI;

public enum InventoryPanelType { None, Backpack, AmmoBelt, Weapons }

public class PlayerInventoryUI : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField]
    protected Inventory _inventory = null;

    [Header("Food Mount")]
    [SerializeField] protected GameObject _foodMount = null;
    [SerializeField] protected Image _foodMountImage = null;
    [SerializeField] protected Text _foodMountName = null;
    [SerializeField] protected Text _foodMountWeight = null;
    [SerializeField] protected Text _foodMountID = null;

    [Header("Weapon Mount")]
    [SerializeField] protected GameObject _weaponMount = null;
    [SerializeField] protected Image _weaponMountImage = null;
    [SerializeField] protected Text _weaponMountName = null;
    [SerializeField] protected Text _weaponMountWeight = null;
    [SerializeField] protected Text _weaponMountID = null;
    [SerializeField] protected Text _weaponMountRounds = null;

    [Header("Ammo Mount")]
    [SerializeField] protected GameObject _ammoMount = null;
    [SerializeField] protected Image _ammoMountImage = null;
    [SerializeField] protected Text _ammoMountName = null;
    [SerializeField] protected Text _ammoMountWeight = null;
    [SerializeField] protected Text _ammoMountID = null;

    [Header("Colors")]
    [SerializeField] protected Color _mountHover = Color.cyan;

    // Internals
    protected Color _mountColor = new Color(0, 0, 0, 0);

    protected InventoryPanelType _selectedPanelType = InventoryPanelType.None;

    public InventoryPanelType selectedPanelType => _selectedPanelType;

    protected virtual void OnEnable()
    {
        Invalidate();
    }

    protected virtual void OnDisable()
    {
    }

    protected virtual void Invalidate()
    {
        _selectedPanelType = InventoryPanelType.None;

        if (_weaponMount != null)
        {
            if (_weaponMountImage != null) _weaponMountImage.sprite = null;
            if (_weaponMountName != null) _weaponMountName.text = "";
            if (_weaponMountWeight != null) _weaponMountWeight.text = "";
            if (_weaponMountID != null) _weaponMountID.text = "";
            if (_weaponMountRounds != null) _weaponMountRounds.text = "";

            _weaponMount.SetActive(false);
            _weaponMount.transform.GetComponent<Image>().fillCenter = false;
        }

        if (_foodMount != null)
        {
            if (_foodMountImage != null) _foodMountImage.sprite = null;
            if (_foodMountName != null) _foodMountName.text = "";
            if (_foodMountWeight != null) _foodMountWeight.text = "";
            if (_foodMountID != null) _foodMountID.text = "";

            _foodMount.SetActive(false);
            _foodMount.transform.GetComponent<Image>().fillCenter = false;
        }

        if (_ammoMount != null)
        {
            if (_ammoMountImage != null) _ammoMountImage.sprite = null;
            if (_ammoMountName != null) _ammoMountName.text = "";
            if (_ammoMountWeight != null) _ammoMountWeight.text = "";
            if (_ammoMountID != null) _ammoMountID.text = "";

            _ammoMount.SetActive(false);
            _ammoMount.transform.GetComponent<Image>().fillCenter = false;
        }

        if (_inventory != null)
        {
            if (_weaponMount != null)
            {
                var weaponMountInfo = _inventory.GetWeapon(0);
                InventoryItemWeapon weapon = null;
                if (weaponMountInfo != null)
                    weapon = weaponMountInfo.Weapon;

                if (weapon != null)
                {
                    if (_weaponMountImage != null) _weaponMountImage.sprite = weapon.Image;
                    if (_weaponMountName != null) _weaponMountName.text = weapon.Name;
                    if (_weaponMountWeight != null) _weaponMountWeight.text = weapon.Weight.ToString() + " kg";
                    if (_weaponMountID != null) _weaponMountID.text = weapon.Identifier;
                    if (_weaponMountRounds != null) _weaponMountRounds.text = weaponMountInfo.Rounds.ToString();

                    _weaponMount.SetActive(true);
                }
            }

            if (_foodMount != null)
            {
                var foodMountInfo = _inventory.GetFood(0);
                InventoryItemFood food = null;
                if (foodMountInfo != null)
                    food = foodMountInfo.Item;

                if (food != null)
                {
                    if (_foodMountImage != null) _foodMountImage.sprite = food.Image;
                    if (_foodMountName != null) _foodMountName.text = food.Name;
                    if (_foodMountWeight != null) _foodMountWeight.text = food.Weight.ToString() + " kg";
                    if (_foodMountID != null) _foodMountID.text = food.Identifier;

                    _foodMount.SetActive(true);
                }
            }

            if (_ammoMount != null)
            {
                var ammoMountInfo = _inventory.GetAmmo(0);
                InventoryItemAmmo ammo = null;
                if (ammoMountInfo != null)
                    ammo = ammoMountInfo.Ammo;

                if (ammo != null)
                {
                    if (_ammoMountImage != null) _ammoMountImage.sprite = ammo.Image;
                    if (_ammoMountName != null) _ammoMountName.text = ammo.Name;
                    if (_ammoMountWeight != null) _ammoMountWeight.text = ammo.Weight.ToString() + " kg";
                    if (_ammoMountID != null) _ammoMountID.text = ammo.Identifier;

                    _ammoMount.SetActive(true);
                }
            }
        }
    }

    public void OnEnterFoodMount(Image image)
    {
        var itemMount = _inventory.GetFood(0);

        if (itemMount == null || itemMount.Item == null) return;

        if (_selectedPanelType != InventoryPanelType.Backpack)
            image.color = _mountHover;

        _selectedPanelType = InventoryPanelType.Backpack;
    }

    public void OnExitFoodMount(Image image)
    {
        if (image != null)
            image.color = _mountColor;

        _selectedPanelType = InventoryPanelType.None;
    }

    public void OnEnterAmmoMount(Image image)
    {
        var itemMount = _inventory.GetAmmo(0);

        if (itemMount == null || itemMount.Ammo == null) return;

        if (_selectedPanelType != InventoryPanelType.AmmoBelt)
            image.color = _mountHover;

        _selectedPanelType = InventoryPanelType.AmmoBelt;
    }

    public void OnExitAmmoMount(Image image)
    {
        if (image == null) return;
        image.color = _mountColor;
        _selectedPanelType = InventoryPanelType.None;
    }

    public void OnEnterWeaponMount(Image image)
    {
        var itemMount = _inventory.GetWeapon(0);

        if (itemMount == null || itemMount.Weapon == null) return;

        if (_selectedPanelType != InventoryPanelType.Weapons)
            image.color = _mountHover;

        _selectedPanelType = InventoryPanelType.Weapons;
    }

    public void OnExitWeaponMount(Image image)
    {
        if (image == null) return;
        image.color = _mountColor;
        _selectedPanelType = InventoryPanelType.None;
    }
}
