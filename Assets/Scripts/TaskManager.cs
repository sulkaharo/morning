using UnityEngine;
using System.Collections;

public class TaskManager : MonoBehaviour
{
	public float LifeTime = 3.0f;

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
		GameManager.Instance.TaskCompleted(gridPosition);
	}


	// Update is called once per frame
	void Update ()
	{
		if (Time.time > creationTime + LifeTime)
		{
			TaskCompleted();
		}
	
	}
}
