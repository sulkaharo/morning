using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TaskManager : TaskManagerBase
{
	public string keyStrokes;
	public GameObject keyButton;
	private int keyNr = 0;
	private GameObject keyButtonGO;
	private Transform keyButtonT;
	private GameObject[] buttons = new GameObject[4];
	
	public override void Start()
	{
		base.Start ();
		
		for (int i = 0; i < keyStrokes.Length; i++) {
			keyButtonGO = GameObject.Instantiate (keyButton);
			GameObject canvas = GameObject.Find ("Canvas");
			if (canvas != null) {
				keyButtonGO.transform.SetParent (canvas.transform, false);
			} else {
				Debug.LogWarning ("no ui canvas");
			}
			
			keyButtonGO.transform.position = transform.position + new Vector3 (-0.6f + 0.65f * (float) i, -0.6f, -6.0f);
			
			TextMesh newTextMesh = keyButtonGO.GetComponentInChildren<TextMesh> ();
			newTextMesh.text = keyStrokes[i].ToString ().ToUpper();
			newTextMesh.transform.position = transform.position + new Vector3 (-0.80f + 0.65f * (float) i, -0.3f, -9.0f);
			buttons[i] = keyButtonGO;
		}
	}
	private void OnDestroy()
	{
		for (int i = 0; i < 4; i++) {
			GameObject.Destroy (buttons[i]);
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
