using UnityEngine;
using System.Collections;

public class TaskManager : TaskManagerBase
{
	public string keyStrokes;

	private int keyNr = 0;

	public override void Start()
	{
		base.Start();

		TextMesh newTextMesh = gameObject.GetComponent<TextMesh>();
		if (newTextMesh != null)
		{
			newTextMesh.text = keyStrokes.ToUpper() + " *" + TotalReps.ToString();
		}

	}

	public override void Update ()
	{
		progress = (float)(keyNr + (repetitionNr * keyStrokes.Length)) / (float)(TotalReps * keyStrokes.Length);

		base.Update();

		if (keyNr == keyStrokes.Length) {
			repetitionNr++;
			keyNr = 0;
			if(repetitionNr == TotalReps) {
				TaskCompleted ();
			}
		} else {
			KeyCode key = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyStrokes[keyNr].ToString(), true);
			
			bool down = Input.GetKeyDown (key);
			//bool held = Input.GetKey (key);
			//bool up = Input.GetKeyUp (key);
			
			if (down) {
				keyNr++;
			}
			/*
			else if (held) {
				keyNr++;
			}*/
			/*else if (up) {
				keyNr++;
			}*/
		}
		
	}
}
