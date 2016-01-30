using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MouseTaskManager : MonoBehaviour
{
	public float LifeTime = 3.0f;
	public float ActionDelay = 1.0f;

	public int Repetitions = 5;
	private int reps = 0;

	public GameObject ButtonPrefab;

	private GameObject buttonGO;
	private Button button;
	private Text text;

	private float creationTime;
	private float lastActivationTime = 0.0f;
	private int gridPosition = -1;

	// Use this for initialization
	void Start ()
	{
		creationTime = Time.time;

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
			text.text = (Repetitions - reps).ToString();
		}
		else
		{
			Debug.LogWarning("no text component in button prefab");
		}
	}
	
	private void ButtonClicked()
	{
		reps++;
		text.text = (Repetitions - reps).ToString();
		lastActivationTime = Time.time;
		ButtonDisable();
	}

	private void ButtonEnable()
	{
		button.interactable = true;
	}

	private void ButtonDisable()
	{
		button.interactable = false;
	}


	public void SetGridPosition (int pos)
	{
		gridPosition = pos;
	}

	private void OnDestroy()
	{
		GameObject.Destroy(buttonGO);
	}

	private void TaskCompleted()
	{
		TaskResult result = new TaskResult();
		result.gridPosition = gridPosition;
		result.completionTime = Time.time - creationTime;
		result.TimeoutTime = LifeTime;
		result.completionPercentage = reps / Repetitions;
		GameManager.Instance.TaskCompleted(result);
	}

	// Update is called once per frame
	void Update ()
	{
		if(reps >= Repetitions)
		{
			TaskCompleted();
		}
		if (Time.time >= lastActivationTime + ActionDelay)
		{
			ButtonEnable();
		}
	}
}
