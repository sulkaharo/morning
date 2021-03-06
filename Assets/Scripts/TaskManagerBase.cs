using UnityEngine;
using System.Collections;

public class TaskManagerBase : MonoBehaviour
{
	public float LifeTime = 10.0f;
	public int TotalReps = 5;
	
	public GameObject expirationBar;
	public GameObject progressBarTemplate;
	private GameObject progressBarGO;
	private GameObject expirationBarGO;
	private Transform expirationT;
	private Transform progressT;
	
	protected float creationTime;
	protected int gridPosition = -1;
	protected float progress = 0.0f;
	protected int repetitionNr = 0;
	
	public virtual void Start ()
	{
		creationTime = Time.time;
		
		GameObject progressBarGO = GameObject.Instantiate(progressBarTemplate) as GameObject;
		progressT = progressBarGO.transform;
		progressBarGO.transform.SetParent(gameObject.transform, false);
		progressT.localPosition = new Vector3(-1.0f, -0.85f, -6.0f);
		
		GameObject expirationBarGO = GameObject.Instantiate (expirationBar) as GameObject;
		expirationT = expirationBarGO.transform;
		expirationBarGO.transform.SetParent(gameObject.transform, false);
		expirationT.localPosition = new Vector3(-1.0f, 1.9f, -6.0f);
	}
	
	public void SetGridPosition (int pos)
	{
		gridPosition = pos;
	}
	
	public void TaskCompleted()
	{
		TaskResult result = new TaskResult();
		result.gridPosition = gridPosition;
		result.completionTime = Time.time - creationTime;
		result.TimeoutTime = LifeTime;
		result.completionPercentage = repetitionNr / TotalReps;
		GameManager.Instance.TaskCompleted(result);
	}
	
	// Update is called once per frame
	public virtual void Update ()
	{
		progressT.localScale = new Vector3(progress * 1.5f, 1.5f, 1.0f);
		//expirationT.localScale = new Vector3 (1.5f - (creationTime - Time.time) * (LifeTime * expBarMultiplier), 1.0f, 1.0f);
		expirationT.localScale = new Vector3 (1.5f - (Time.time - creationTime) / LifeTime * 1.5f, 0.6f, 1.0f);
		
		if (Time.time > creationTime + LifeTime)
		{
			TaskCompleted();
		}
	}
}
