using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MouseTaskManager : TaskManagerBase
{
	public float ActionDelay = 1.0f;

	public GameObject ButtonPrefab;

	private GameObject buttonGO;
	private Button button;
	private Text text;

	private float lastActivationTime = 0.0f;

	// Use this for initialization
	public override void Start ()
	{
		base.Start();

		buttonGO = GameObject.Instantiate(ButtonPrefab);
		GameObject canvas = GameObject.Find("Canvas");
		if (canvas != null)
		{
			buttonGO.transform.SetParent(canvas.transform, false);
		}
		else
		{
			Debug.LogWarning("no ui canvas");
		}

		buttonGO.transform.position = transform.position + new Vector3(0.5f, -0.5f, 0.0f);

		button = buttonGO.GetComponent<Button>();
		if (button != null)
		{
			button.onClick.AddListener(ButtonClicked);
		}
		else
		{
			Debug.LogWarning("no button component in button prefab");
		}

		text = buttonGO.GetComponentInChildren<Text>();
		if (text != null)
		{
			text.text = (TotalReps - repetitionNr).ToString();
		}
		else
		{
			Debug.LogWarning("no text component in button prefab");
		}
	}
	
	private void ButtonClicked()
	{
		repetitionNr++;
		
		lastActivationTime = Time.time;
		ButtonDisable();
	}

	private void ButtonEnable()
	{
		button.interactable = true;
		text.text = (TotalReps - repetitionNr).ToString();
	}

	private void ButtonDisable()
	{
		button.interactable = false;
	}



	private void OnDestroy()
	{
		GameObject.Destroy(buttonGO);
	}
/*
	private void TaskCompleted()
	{
		TaskResult result = new TaskResult();
		result.gridPosition = gridPosition;
		result.completionTime = Time.time - creationTime;
		result.TimeoutTime = LifeTime;
		result.completionPercentage = reps / Repetitions;
		GameManager.Instance.TaskCompleted(result);
	}
	*/

	// Update is called once per frame
	public override void Update ()
	{
		progress = (float) repetitionNr / (float) TotalReps;

		base.Update();

		if(repetitionNr >= TotalReps)
		{
			TaskCompleted();
		}

		if (Time.time >= lastActivationTime + ActionDelay)
		{
			ButtonEnable();
		}
		if(button.interactable == false)
		{
			text.text = (1.0f - ((Time.time - lastActivationTime)/ActionDelay)).ToString();
		}
	}
}
