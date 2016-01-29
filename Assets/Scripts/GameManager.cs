using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	public GameObject[] TaskParents;

	//Official Game TIme
	public float GameTime { get; private set; }
	public float SpawnInterval = 2.0f;

	private float startTime, elapsedTime, lastSpawnTime;

	private TasksResource Tasks;

	private List<GameObject> ActiveTasks = new List<GameObject>();

	private GridDefinition Grid;


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

		Grid = new GridDefinition();

		Debug.Log(Tasks.MorningTasks[0].name);
	}

	void Start()
	{
		/*
		for (int i = 0; i < TaskParents.Length; i++)
		{
			ActiveTasks.Add(GameObject.Instantiate(Tasks.MorningTasks[Random.Range(0, Tasks.MorningTasks.Length)]));
			//Bounds bbox = ActiveTasks[i].transform.
			ActiveTasks[i].transform.parent = TaskParents[i].transform;
			ActiveTasks[i].transform.localPosition = Vector3.zero;
		}
		*/
		lastSpawnTime = 0.0f;
	}

	//Main game loop. Could be FixedUpdate() too?
	void Update ()
	{
		GameTime = Time.time - startTime;

		if (GameTime > lastSpawnTime + SpawnInterval)
		{
			GameObject randomTask = Tasks.MorningTasks[Random.Range(0, Tasks.MorningTasks.Length)];
			GameObject newTask = GameObject.Instantiate(randomTask);
			ActiveTasks.Add(newTask);
			int nextEmptyGridPosition = Grid.GetNextEmpty();
			Grid.TaskInPosition[nextEmptyGridPosition] = newTask;
			newTask.transform.parent = TaskParents[nextEmptyGridPosition].transform;
			newTask.transform.localPosition = Vector3.zero;

			TaskManager newTaskManager = newTask.GetComponent<TaskManager>();
			if(newTaskManager != null)
			{
				newTaskManager.SetGridPosition(nextEmptyGridPosition);
			}

			Debug.Log("Spawned task " + newTask.name + " at grid position " + nextEmptyGridPosition);
			lastSpawnTime = GameTime;
		}
	}

	public void TaskCompleted(int pos)
	{
		GameObject completedTask = Grid.TaskInPosition[pos];
		GameObject.Destroy(completedTask);
		Grid.TaskInPosition[pos] = null;
	}


}
