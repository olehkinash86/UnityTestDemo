using UnityEngine;

public enum ActivationType { MouseButtonPressed, MouseButtonDown }

public class InteractiveItem : MonoBehaviour
{
    [SerializeField] protected int _priority = 0;
    public int Priority => _priority;

    [SerializeField] protected ActivationType _activationType = ActivationType.MouseButtonDown;
    public ActivationType activationType => _activationType;

    protected GameSceneManager _gameSceneManager = null;
    protected Collider _collider = null;
    protected Rigidbody _rigidBody = null;

    public virtual void Drag(CharacterManager characterManager) { }
    public virtual void Drop() { }
    public virtual void Attach() { }

    public virtual Vector3 GetAttachPosition()
    {
        return new Vector3(0, 0, 0);
    }

    public virtual Quaternion GetAttachRotation()
    {
        return Quaternion.Euler(0, 0, 0);
    }

    protected virtual void Start()
    {
        _gameSceneManager = GameSceneManager.Instance;
        _collider = GetComponent<Collider>();
        _rigidBody = GetComponent<Rigidbody>();

        if (_gameSceneManager != null && _collider != null)
        {
            _gameSceneManager.RegisterInteractiveItem(_collider.GetInstanceID(), this);
        }
    }
}
