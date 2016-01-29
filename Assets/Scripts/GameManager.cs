using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	public GameObject[] TaskParents;

	//Official Game TIme
	public float GameTime { get; private set; }

	private float startTime, elapsedTime;

	private TasksResource Tasks;

	private List<GameObject> ActiveTasks = new List<GameObject>();

	void Awake ()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		Instance = this;

		DontDestroyOnLoad(gameObject);

		startTime = Time.time;

		Tasks = Resources.Load<TasksResource>("Tasks");

		Debug.Log(Tasks.MorningTasks[0].name);
	}

	void Start()
	{
		for (int i = 0; i < TaskParents.Length; i++)
		{
			ActiveTasks.Add(GameObject.Instantiate(Tasks.MorningTasks[Random.Range(0, Tasks.MorningTasks.Length)]));
			//Bounds bbox = ActiveTasks[i].transform.
			ActiveTasks[i].transform.parent = TaskParents[i].transform;
			ActiveTasks[i].transform.localPosition = Vector3.zero;
		}
	}


	//Main game loop. Could be FixedUpdate() too?
	void Update ()
	{
		elapsedTime = Time.time - startTime;
		GameTime = elapsedTime;

	}
}
