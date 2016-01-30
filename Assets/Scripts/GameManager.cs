using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	public GameObject[] TaskParents;

	public GameObject HygieneMeter;
	private Transform HygieneMeterTransform;

	public GameObject GameOverPrefab;
	public GameObject TaskCompleteFX;

	//Official Game TIme
	public float GameTime { get; private set; }
	public float SpawnInterval = 2.0f;

	private float startTime, elapsedTime, lastSpawnTime;

	private TasksResource Tasks;

	private List<GameObject> ActiveTasks = new List<GameObject>();

	private GridDefinition Grid;

	private float HygieneLevel = 50.0f;
	private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

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

				string randomString = GetRandomString(2);
				TaskManager newTaskManager = newTask.GetComponent<TaskManager>();
				if (newTaskManager != null)
				{
					newTaskManager.SetGridPosition(nextEmptyGridPosition);
					newTaskManager.keyStrokes = randomString;
				}
				TextMesh newTextMesh = newTask.GetComponent<TextMesh>();
				if (newTextMesh != null)
				{
					newTextMesh.text = randomString;
				}

				Debug.Log("Spawned task " + newTask.name + " at grid position " + nextEmptyGridPosition);
				lastSpawnTime = GameTime;
				SpawnInterval *= 0.95f;
			}
			else
			{
				GameOver();
			}
		}
		Debug.Log("Active tasks " + ActiveTasks.Count);
		HygieneLevel += (Random.value - 0.6f) * 0.5f;
		HygieneMeterTransform.localScale = new Vector3(1.0f, HygieneLevel / 10.0f, 1.0f);
	}

	public void TaskCompleted(TaskResult result)
	{
		GameObject completedTask = Grid.TaskInPosition[result.gridPosition];
		GameObject.Destroy(completedTask);
		ActiveTasks.Remove(completedTask);
		Grid.TaskInPosition[result.gridPosition] = null;
		Debug.Log("Task at " + result.gridPosition + " completed in " + result.completionTime + " seconds");

		GameObject FX = GameObject.Instantiate(TaskCompleteFX);

		FX.transform.parent = TaskParents[result.gridPosition].transform;
		FX.transform.localPosition = Vector3.zero;
		HygieneLevel += 5.0f;
	}

	private void GameOver()
	{
		GameOverPrefab.SetActive(true);
	}

	private string GetRandomString(int length)
	{
		string str = "";
		for (int i = 0; i < length; i++)
		{
			str += chars[(int)(Random.value * 26)].ToString();
		}
		return str;
	}
}


public class TaskResult
{
	public int gridPosition;
	public float completionTime;
	public float completionPercentage;
}
