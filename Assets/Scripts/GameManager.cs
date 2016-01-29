using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	public GameObject[] TaskParents;

	//Official Game TIme
	public float GameTime { get; private set; }

	private float startTime, elapsedTime;

	void Awake ()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}

		Instance = this;

		DontDestroyOnLoad(gameObject);

		startTime = Time.time;
	}

	//Main game loop. Could be FixedUpdate() too?
	void Update ()
	{
		elapsedTime = Time.time - startTime;
		GameTime = elapsedTime;

	}
}
