using UnityEngine;
using System.Collections;

public class MouseTaskManager : MonoBehaviour
{
	public float LifeTime = 3.0f;
	//public string keyStrokes;
	//public int keyNr = 0;

	private float creationTime;
	private int gridPosition = -1;

	// Use this for initialization
	void Start ()
	{
		creationTime = Time.time;		
	}
	
	public void SetGridPosition (int pos)
	{
		gridPosition = pos;
	}

	private void TaskCompleted()
	{
		TaskResult result = new TaskResult();
		result.gridPosition = gridPosition;
		result.completionTime = Time.time - creationTime;
		GameManager.Instance.TaskCompleted(result);
	}

	// Update is called once per frame
	void Update ()
	{


		/*
		if (Time.time > creationTime + LifeTime)
		{
			TaskCompleted();
		}
	*/
	}
}
