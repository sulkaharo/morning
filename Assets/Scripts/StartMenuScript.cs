using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartMenuScript : MonoBehaviour {

	public Button startButton;
	public Button exitButton;

	// Use this for initialization
	void Start () {
		startButton = startButton.GetComponent <Button> ();
		exitButton = exitButton.GetComponent <Button> ();
	}
	
	public void ExitPress() 
	{
		Application.Quit ();
	}

	public void StartGame()
	{
		Application.LoadLevel ("Main");

	}

}
