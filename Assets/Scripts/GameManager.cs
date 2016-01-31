using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	public GameObject[] TaskParents;

	public GameObject GameOverPrefab;
	public GameObject NewGamePrefab;
	public GameObject TaskCompleteFX;
	public GameObject TaskIncompleteFX;

	public GameObject TutorialTask;

	public GameObject ScoreObject, HighScoreObject, CompletedTasksObject;
	private UnityEngine.UI.Text scoreText, highScoreText, completedTasksText;

	//Official Game TIme
	public float GameTime { get; private set; }
	public float SpawnInterval = 2.0f;

	private float startTime, elapsedTime, lastSpawnTime;
	private int completedTasks = 0;
	private int score = 0;
	private float combinedTime;

	private int day = 0;
	private int dataDay = 0; // the day we access from data, might be less than the actual day if there is not enough data. 
	private int hours, minutes;

	private TasksResource Tasks;

	private List<GameObject> ActiveTasks = new List<GameObject>();
	private GameObject ClockGO;
	private TextMesh ClockText;
	private GameObject DayGO;
	private TextMesh DayText;
	private GameObject TimeBarGO;

	private GridDefinition Grid;

	private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
	private static string highScoreFilename = "highscore.txt";
	private static int dayStart = 6;
	private static int dayEnd = 8;
	private bool gameOver = false;
	private int highScore = 0;

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
		TimeBarGO = GameObject.Find("TimeBar");

		scoreText = ScoreObject.GetComponent<UnityEngine.UI.Text>();
		highScoreText = HighScoreObject.GetComponent<UnityEngine.UI.Text>();
		completedTasksText = CompletedTasksObject.GetComponent<UnityEngine.UI.Text>();

		highScore = ReadHighScore();

		highScoreText.text = "High Score: " + highScore.ToString();

		lastSpawnTime = 0.0f;
	}

	void Start()
	{
		scoreText.text = "Score: " + score.ToString();
		//spawn tutorial task
		GameObject FTUETask = GameObject.Instantiate(TutorialTask);
		ActiveTasks.Add(FTUETask);
		Grid.TaskInPosition[4] = FTUETask;
		FTUETask.transform.parent = TaskParents[4].transform;
		FTUETask.transform.localPosition = Vector3.zero;

		TaskManagerBase newTaskManager = FTUETask.GetComponent<TaskManagerBase>();
		if (newTaskManager != null)
		{
			newTaskManager.SetGridPosition(4);
		}
	}

	//Main game loop. 
	void Update ()
	{
		GameTime = Time.time - startTime;

		int rawMinutes = (int) System.Math.Floor(GameTime * 3.0f);

		minutes = rawMinutes % 60;
		hours = dayStart + (rawMinutes - minutes) / 60;
		ClockText.text = hours.ToString("D2") + ":" + minutes.ToString("D2");

		if(hours >= dayEnd && !gameOver)
		{
			NextDay();
		}

		if (GameTime > lastSpawnTime + SpawnInterval && !gameOver)
		{
			
			int nextEmptyGridPosition = Grid.GetNextEmpty();
			if (nextEmptyGridPosition != -1)
			{
				GameObject randomTask = Tasks.Days[dataDay].Tasks[Random.Range(0, Tasks.Days[dataDay].Tasks.Length)];
				GameObject newTask = GameObject.Instantiate(randomTask);

				ActiveTasks.Add(newTask);
				Grid.TaskInPosition[nextEmptyGridPosition] = newTask;
				newTask.transform.parent = TaskParents[nextEmptyGridPosition].transform;
				newTask.transform.localPosition = Vector3.zero;

				TaskManagerBase newTaskManager = newTask.GetComponent<TaskManagerBase>();
				if (newTaskManager != null)
				{
					newTaskManager.SetGridPosition(nextEmptyGridPosition);
				}

				lastSpawnTime = GameTime;
				SpawnInterval *= 0.96f;
			}
			else if(!gameOver)
			{
				GameOver();
			}
		}

		TimeBarGO.transform.localScale = new Vector3(1.0f - rawMinutes / (60.0f*(dayEnd-dayStart) ), 1.0f, 1.0f );
	}

	public void TaskCompleted(TaskResult result)
	{
		GameObject completedTask = Grid.TaskInPosition[result.gridPosition];
		GameObject.Destroy(completedTask);
		ActiveTasks.Remove(completedTask);
	
		Grid.TaskInPosition[result.gridPosition] = null;
		//Debug.Log("Task at " + result.gridPosition + " completed in " + result.completionTime + " seconds");

		bool success = result.completionPercentage > 0.99f;

		GameObject FX = success ? GameObject.Instantiate(TaskCompleteFX) : GameObject.Instantiate(TaskIncompleteFX);

		FX.transform.parent = TaskParents[result.gridPosition].transform;
		FX.transform.localPosition = Vector3.zero;

		//update scores
		if (success) completedTasks++;
		combinedTime += result.completionTime;

		int taskScore = (int) (10.0f * (result.TimeoutTime / (result.completionTime + 0.01f)));
		if(success) score += taskScore;
		scoreText.text = "Score: " + score.ToString();
		completedTasksText.text = "Completed: " + completedTasks.ToString();

		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.PlaySound(SoundEffect.TaskComplete);
		}

	}

	private void NextDay()
	{
		day++;
		startTime = Time.time;
		lastSpawnTime = 0.0f;
		GameTime = Time.time - startTime;

		dataDay = System.Math.Min(day, Tasks.Days.Length-1);

		SpawnInterval = Tasks.Days[dataDay].SpawnInterval;
		KillAllTasks();

		DayText.text = "Day " + (day+1).ToString(); // +1 because the first day is day 1, not day 0
		GameObject.Instantiate(Tasks.Days[dataDay].DayStartBillboard);

	}

	private int ReadHighScore()
	{
		int highscore = PlayerPrefs.GetInt("highscore");
		return highscore;
	}

	private void WriteHighScore(int newhighscore)
	{
		PlayerPrefs.SetInt ("highscore", newhighscore);
	}

	private void GameOver()
	{
		GameOverPrefab.SetActive(true);
		NewGamePrefab.SetActive(true);
		KillAllTasks();
		gameOver = true;

		if (score > highScore)
		{
			highScore = score;
			WriteHighScore(score);
			highScoreText.text = "High Score: " + highScore.ToString();
		}
	}

	public void NewGame()
	{
		score = 0;
		day = dataDay = 0;
		startTime = Time.time;
		GameTime = lastSpawnTime = 0.0f;
		gameOver = false;

		SpawnInterval = Tasks.Days[0].SpawnInterval;

		GameOverPrefab.SetActive(false);
		NewGamePrefab.SetActive(false);

		DayText.text = "Day " + (day + 1).ToString(); // +1 because the first day is day 1, not day 0
		Start();
	}

	private void KillAllTasks()
	{
		for (int i = 0; i < 9; i++)
		{
			GameObject completedTask = Grid.TaskInPosition[i];
			if (completedTask != null)
			{
				GameObject.Destroy(completedTask);
				ActiveTasks.Remove(completedTask);
			}
		}
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
