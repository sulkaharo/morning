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

	public GameObject ScoreObject, CombinedTimeObject, CompletedTasksObject;
	private UnityEngine.UI.Text scoreText, combinedTimeText, completedTasksText;

	//Official Game TIme
	public float GameTime { get; private set; }
	public float SpawnInterval = 2.0f;

	private float startTime, elapsedTime, lastSpawnTime;
	private int completedTasks = 0;
	private int score = 0;
	private float combinedTime;

	private int day = 0;
	private int hours, minutes;

	private TasksResource Tasks;

	private List<GameObject> ActiveTasks = new List<GameObject>();
	private GameObject ClockGO;
	private TextMesh ClockText;
	private GameObject DayGO;
	private TextMesh DayText;

	private GridDefinition Grid;

	private float HygieneLevel = 50.0f;
	private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
	private static int dayStart = 6;
	private static int dayEnd = 8;

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

		ClockGO = GameObject.Find("Time");
		ClockText = ClockGO.GetComponent<TextMesh>();
		DayGO = GameObject.Find("Day");
		DayText = DayGO.GetComponent<TextMesh>();

		scoreText = ScoreObject.GetComponent<UnityEngine.UI.Text>();
		combinedTimeText = CombinedTimeObject.GetComponent<UnityEngine.UI.Text>();
		completedTasksText = CompletedTasksObject.GetComponent<UnityEngine.UI.Text>();

		lastSpawnTime = 0.0f;
		HygieneMeterTransform = HygieneMeter.transform;
	}

	//Main game loop. 
	void Update ()
	{

		GameTime = Time.time - startTime;

		int rawMinutes = (int) System.Math.Floor(GameTime * 3.0f);

		minutes = rawMinutes % 60;
		hours = dayStart + (rawMinutes - minutes) / 60;
		ClockText.text = hours.ToString("D2") + ":" + minutes.ToString("D2");

		if(hours >= dayEnd)
		{
			NextDay();
		}

		if (GameTime > lastSpawnTime + SpawnInterval)
		{
			
			int nextEmptyGridPosition = Grid.GetNextEmpty();
			if (nextEmptyGridPosition != -1)
			{
				GameObject randomTask = Tasks.Days[day].Tasks[Random.Range(0, Tasks.Days[day].Tasks.Length)];
				GameObject newTask = GameObject.Instantiate(randomTask);

				ActiveTasks.Add(newTask);
				Grid.TaskInPosition[nextEmptyGridPosition] = newTask;
				newTask.transform.parent = TaskParents[nextEmptyGridPosition].transform;
				newTask.transform.localPosition = Vector3.zero;

				//string randomString = GetRandomString(2);
				//Repetitioiden määrä
				int randomInt = Random.Range(1,5);

				TaskManager newTaskManager = newTask.GetComponent<TaskManager>();
				if (newTaskManager != null)
				{
					newTaskManager.SetGridPosition(nextEmptyGridPosition);
					//newTaskManager.keyStrokes = randomString;
					newTaskManager.totalReps = randomInt;
				}

				MouseTaskManager newMouseTaskManager = newTask.GetComponent<MouseTaskManager>();
				if (newMouseTaskManager != null)
				{
					newMouseTaskManager.SetGridPosition(nextEmptyGridPosition);
				}

				MouseSliderTaskManager newMouseSliderTaskManager = newTask.GetComponent<MouseSliderTaskManager>();
				if (newMouseSliderTaskManager != null)
				{
					newMouseSliderTaskManager.SetGridPosition(nextEmptyGridPosition);
				}
				
				TextMesh newTextMesh = newTask.GetComponent<TextMesh>();
				if (newTextMesh != null && newTaskManager != null)
				{
					//newTextMesh.text = randomString + " " + randomInt.ToString();
					newTextMesh.text = newTaskManager.keyStrokes.ToUpper() + " *" + randomInt.ToString();
				}

				//Debug.Log("Spawned task " + newTask.name + " at grid position " + nextEmptyGridPosition);
				lastSpawnTime = GameTime;
				SpawnInterval *= 0.96f;
			}
			else
			{
				GameOver();
			}
		}
		//Debug.Log("Active tasks " + ActiveTasks.Count);
		HygieneLevel += (Random.value - 0.6f) * 0.3f;
		HygieneMeterTransform.localScale = new Vector3(1.0f, HygieneLevel / 10.0f, 1.0f);
	}

	public void TaskCompleted(TaskResult result)
	{
		GameObject completedTask = Grid.TaskInPosition[result.gridPosition];
		GameObject.Destroy(completedTask);
		ActiveTasks.Remove(completedTask);

	
		Grid.TaskInPosition[result.gridPosition] = null;
		//Debug.Log("Task at " + result.gridPosition + " completed in " + result.completionTime + " seconds");

		GameObject FX = GameObject.Instantiate(TaskCompleteFX);

		FX.transform.parent = TaskParents[result.gridPosition].transform;
		FX.transform.localPosition = Vector3.zero;
		HygieneLevel += 5.0f;

		//update scores
		completedTasks++;
		combinedTime += result.completionTime;
		int taskScore = (int) (10.0f * (result.TimeoutTime / (result.completionTime + 0.01f)));
		score += taskScore;
		scoreText.text = "Score: " + score.ToString();
		combinedTimeText.text = "Task time: " + combinedTime.ToString();
		completedTasksText.text = "Completed: " + completedTasks.ToString();

	}

	private void NextDay()
	{
		day++;
		startTime = Time.time;
		lastSpawnTime = 0.0f;
		GameTime = Time.time - startTime;

		SpawnInterval = Tasks.Days[day].SpawnInterval;

		for (int i = 0; i < 9; i++)
		{
			GameObject completedTask = Grid.TaskInPosition[i];
			if (completedTask != null)
			{
				GameObject.Destroy(completedTask);
				ActiveTasks.Remove(completedTask);
			}
		}
		DayText.text = "Day " + (day+1).ToString(); // +1 because the first day is day 1, not day 0
		GameObject dayGO = GameObject.Instantiate(Tasks.Days[day].DayStartBillboard) as GameObject;
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
	public float TimeoutTime;

	public float completionPercentage; // 0...1
}
