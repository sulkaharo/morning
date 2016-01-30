using UnityEngine;
using System.Collections;

public class TaskManager : MonoBehaviour
{
	public float LifeTime = 10.0f;
	public string keyStrokes;
	public int keyNr = 0;
	public int repetitionNr = 0;
	public int totalReps;
	
	public GameObject expiriationBar;
	public GameObject progressBarTemplate;
	private GameObject progressBarGO;
	private GameObject expiriationBarGO;
	private Transform expiriationT;
	private Transform progressT;
	private float creationTime;
	private int gridPosition = -1;
	private float expBarMultiplier =-0.1666f;
	
	// Use this for initialization
	void Start ()
	{
		creationTime = Time.time;
		GameObject progressBarGO = GameObject.Instantiate(progressBarTemplate) as GameObject;
		progressT = progressBarGO.transform;
		progressBarGO.transform.SetParent(gameObject.transform, false);
		progressT.localPosition = new Vector3(-1.0f, -0.85f, -6.0f);
		
		GameObject expiriationBarGO = GameObject.Instantiate (expiriationBar) as GameObject;
		expiriationT = expiriationBarGO.transform;
		expiriationBarGO.transform.SetParent(gameObject.transform, false);
		expiriationT.localPosition = new Vector3(-1.0f, 1.85f, -6.0f);
		
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
		progressT.localScale = new Vector3( (float)(keyNr + (repetitionNr * keyStrokes.Length)) / (float) (totalReps* keyStrokes.Length) * (float)1.5, 1.5f, 1.0f);
		expiriationT.localScale = new Vector3 (1.5f - (creationTime - Time.time) * (LifeTime * expBarMultiplier), 1.0f, 1.0f);
		
		if (keyNr == keyStrokes.Length) {
			repetitionNr++;
			keyNr = 0;
			if(repetitionNr == totalReps) {
				TaskCompleted ();
			}
		} else {
			KeyCode key = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyStrokes[keyNr].ToString(), true);
			
			Debug.Log(key.ToString() + " sought");
			
			bool down = Input.GetKeyDown (key);
			bool held = Input.GetKey (key);
			//bool up = Input.GetKeyUp (key);
			
			if (down) {
				keyNr++;
			} else if (held) {
				keyNr++;
			} /*else if (up) {
				keyNr++;
			}*/
		}
		
		if (Time.time > creationTime + LifeTime)
		{
			TaskCompleted();
		}
		
	}
}
