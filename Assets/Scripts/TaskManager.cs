using UnityEngine;
using System.Collections;

public class TaskManager : MonoBehaviour
{
	public float LifeTime = 3.0f;
	public string keyStrokes;
	public int keyNr = 0;

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
		if (keyNr == keyStrokes.Length) {
			TaskCompleted ();
		} else {
			KeyCode key = (KeyCode)System.Enum.Parse (typeof(KeyCode), keyStrokes[keyNr].ToString ());

			bool down = Input.GetKeyDown (key);
			bool held = Input.GetKey (key);
			bool up = Input.GetKeyUp (key);

			if (down) {
				keyNr++;
			} else if (held) {
				keyNr++;
			} else if (up) {
				keyNr++;
			}
		}
		if (Time.time > creationTime + LifeTime)
		{
			TaskCompleted();
		}
	
	}
}
