using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	public GameObject[] TaskParents;

	public GameObject HygieneMeter;
	private Transform HygieneMeterTransform;

	//Official Game TIme
	public float GameTime { get; private set; }
	public float SpawnInterval = 2.0f;

	private float startTime, elapsedTime, lastSpawnTime;

	private TasksResource Tasks;

	private List<GameObject> ActiveTasks = new List<GameObject>();

	private GridDefinition Grid;

	private float HygieneLevel = 50.0f;

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

		lastSpawnTime = 0.0f;
		HygieneMeterTransform = HygieneMeter.transform;

		//Debug.Log(Tasks.MorningTasks[0].name);
	}

	//Main game loop. 
	void Update ()
	{
		GameTime = Time.time - startTime;

		if (GameTime > lastSpawnTime + SpawnInterval)
		{
			int nextEmptyGridPosition = Grid.GetNextEmpty();
			if (nextEmptyGridPosition != -1)
			{
				GameObject randomTask = Tasks.MorningTasks[Random.Range(0, Tasks.MorningTasks.Length)];
				GameObject newTask = GameObject.Instantiate(randomTask);

				ActiveTasks.Add(newTask);
				Grid.TaskInPosition[nextEmptyGridPosition] = newTask;
				newTask.transform.parent = TaskParents[nextEmptyGridPosition].transform;
				newTask.transform.localPosition = Vector3.zero;

				TaskManager newTaskManager = newTask.GetComponent<TaskManager>();
				if (newTaskManager != null)
				{
					newTaskManager.SetGridPosition(nextEmptyGridPosition);
				}

				Debug.Log("Spawned task " + newTask.name + " at grid position " + nextEmptyGridPosition);
				lastSpawnTime = GameTime;
			}
		}
		Debug.Log("Active tasks " + ActiveTasks.Count);
		HygieneLevel += (Random.value - 0.5f) * 1.0f;
		HygieneMeterTransform.localScale = new Vector3(1.0f, HygieneLevel / 10.0f, 1.0f);
	}

	public void TaskCompleted(int pos)
	{
		GameObject completedTask = Grid.TaskInPosition[pos];
		ActiveTasks.Remove(completedTask);
		GameObject.Destroy(completedTask);
		Grid.TaskInPosition[pos] = null;
	}

}

public class TaskResult
{
	int gridPosition;
	float completionTime;
	float completionPercentage;
}
