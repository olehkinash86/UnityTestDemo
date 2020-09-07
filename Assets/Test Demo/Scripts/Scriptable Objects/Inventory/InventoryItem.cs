using UnityEngine;

public enum InventoryItemType { None, Ammunition, Consumable, Weapon }

[CreateAssetMenu(menuName = "Scriptable Objects/Inventory System/Items/Base")]
public class InventoryItem : ScriptableObject
{
    [SerializeField] protected string _name = null;
    [SerializeField] protected Sprite _image = null;
    [SerializeField] protected float _weight = 0;
    [SerializeField] protected string _identifier = null;

    [Tooltip("The collectable Item that is instantiated in the scene when this item is dropped from the Inventory.")]
    [SerializeField] protected CollectableItem _collectableItem = null;

    [Tooltip("The type of Inventory Item this is.")]
    [SerializeField] protected InventoryItemType _category = InventoryItemType.None;

    // Property Getters
    public string Name => _name;
    public Sprite Image => _image;
    public float Weight => _weight;
    public string Identifier => _identifier;
    public InventoryItemType Category => _category;

    public virtual void Pickup(Vector3 position)
    {
    }

    public virtual CollectableItem Drop(Vector3 position)
    {
        if (_collectableItem != null)
        {
            CollectableItem go = Instantiate<CollectableItem>(_collectableItem);
            go.transform.position = position;
            return go;
        }

        return null;
    }
}
