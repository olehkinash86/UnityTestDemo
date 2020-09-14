using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterManager : MonoBehaviour {
	[SerializeField] private Camera _camera = null;

    [Header("Inventory")]
	[SerializeField] private Inventory _inventory = null;
	
	private GameSceneManager _gameSceneManager = null;
	private int _interactiveMask = 0;
    private int _backpackMask = 0;

	public CollectableItem SelectedDraggableItem = null;
	public CollectableItem SelectedStorageItem = null;

	void Start () {
		_gameSceneManager = GameSceneManager.Instance;
		_interactiveMask	= 1 << LayerMask.NameToLayer("Interactive");
        _backpackMask = 1 << LayerMask.NameToLayer("Backpack");

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

	}
    private void OnEnable()
    {
        if (_inventory)
        {
            _inventory.OnItemDropped.AddListener(OnDropItem);
			_inventory.OnItemAdded.AddListener(OnAddItem);
		}
    }

    private void OnDisable()
    {
        if (_inventory)
        {
            _inventory.OnItemDropped.RemoveListener(OnDropItem);
			_inventory.OnItemAdded.RemoveListener(OnAddItem);
		}
    }

	private void OnDropItem(InventoryItem item)
    {
		StartCoroutine(SendRequestToServer(item, "drop"));
    }

	private void OnAddItem(InventoryItem item)
	{
		StartCoroutine(SendRequestToServer(item, "add"));
	}

	IEnumerator SendRequestToServer(InventoryItem item, string action)
    {
		var url = "https://dev3r02.elysium.today/inventory/status";
		var postData = new Dictionary<string, string> {
			{"name", item.Name },
			{"action", action },
			{"Category", nameof(item.Category) },
		};

		UnityWebRequest www = UnityWebRequest.Post(url, postData);
        www.SetRequestHeader("auth", "BMeHG5xqJeB4qCjpuJCTQLsqNGaqkfB6");

        yield return www.SendWebRequest();

		if (www.isNetworkError || www.isHttpError)
			Debug.Log(www.error);
		else
			Debug.Log($"{postData["name"]}, {postData["action"]}, {postData["Category"]}, " + "response:"+www.downloadHandler.text);
    }

	void Update()
	{
        // PROCESS INTERACTIVE OBJECTS
		// Is the crosshair over a usuable item or descriptive item...first get ray from centre of screen
		var ray = _camera.ScreenPointToRay( new Vector3(Screen.width/2, Screen.height/2, 0));

		// Cast Ray and collect ALL hits
		var hits = Physics.RaycastAll (ray, 5, _interactiveMask | _backpackMask);

		// Process the hits for the one with the highest priority
		if (hits.Length>0)
		{
			// Used to record the index of the highest priority
			int 				highestPriority = int.MinValue;
			InteractiveItem		priorityObject	= null;	

			// Iterate through each hit
			foreach (var hit in hits)
            {
                // Fetch its InteractiveItem script from the database
                InteractiveItem interactiveObject = _gameSceneManager.GetInteractiveItem( hit.collider.GetInstanceID());

                // If this is the highest priority object so far then remember it
                if (interactiveObject!=null && interactiveObject.Priority>highestPriority)
                {
                    priorityObject = interactiveObject;
                    highestPriority= priorityObject.Priority;
                }
            }

			if (priorityObject!=null)
			{
				if (priorityObject.activationType == ActivationType.MouseButtonPressed && Input.GetMouseButton(0) ||
					priorityObject.activationType == ActivationType.MouseButtonDown && Input.GetMouseButtonDown(0))
				{
					priorityObject.Drag( this );
				}
			}
		}

		if(SelectedDraggableItem != null)
        {
			SelectedDraggableItem.Drag(this);
            if (Input.GetMouseButtonUp(0))
            {
                SelectedDraggableItem.Drop();
                SelectedDraggableItem = null;
            }
		}

        if (SelectedStorageItem != null && Input.GetMouseButtonUp(0))
        {
            SelectedStorageItem.Drop();
			SelectedStorageItem = null;
		}
    }
}
