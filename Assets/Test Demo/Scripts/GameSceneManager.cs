using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour {
	private static GameSceneManager _instance = null;
	public static GameSceneManager Instance
	{
		get
		{
			if (_instance == null)
				_instance = (GameSceneManager)FindObjectOfType(typeof(GameSceneManager));
			return _instance;
		}
	}
	
	private readonly Dictionary< int, InteractiveItem>		_interactiveItems	=	new Dictionary<int, InteractiveItem>();

	public void RegisterInteractiveItem( int key, InteractiveItem script )
	{
		if (!_interactiveItems.ContainsKey( key ))
		{
			_interactiveItems[key] = script;
		}
	}

	public InteractiveItem GetInteractiveItem( int key )
	{
        _interactiveItems.TryGetValue(key, out var item);
		return item;
	}

}
