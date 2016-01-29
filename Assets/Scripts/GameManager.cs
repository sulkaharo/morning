using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

	public static GameManager Instance { get; private set; }

	void Awake ()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}
	

	void Update ()
	{
	
	}
}
