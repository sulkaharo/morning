using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MouseSliderTaskManager : TaskManagerBase
{
	public int Repetitions = 5;
	private int reps = 0;

	public GameObject SliderPrefab;

	private GameObject sliderGO;
	private Slider slider;
	private Text text;

	// Use this for initialization
	public override void Start ()
	{
		base.Start();

		sliderGO = GameObject.Instantiate(SliderPrefab);
		GameObject canvas = GameObject.Find("Canvas");
		if (canvas != null)
		{
			sliderGO.transform.SetParent(canvas.transform, false);
		}
		else
		{
			Debug.LogWarning("no ui canvas");
		}

		sliderGO.transform.position = transform.position + new Vector3(0.5f, -0.5f, 0.0f);

		slider = sliderGO.GetComponent<Slider>();
		if (slider != null)
		{
			slider.onValueChanged.AddListener(SliderChanged);
		}
		else
		{
			Debug.LogWarning("no slider component in slider prefab");
		}

		text = sliderGO.GetComponentInChildren<Text>();
		if (text != null)
		{
			text.text = (Repetitions - reps).ToString();
		}
		else
		{
			Debug.LogWarning("no text component in slider prefab");
		}
	}

	private void SliderChanged(float val)
	{
		if (val > 0.99f)
		{
			reps++;
		}
	}

	private void OnDestroy()
	{
		GameObject.Destroy(sliderGO);
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
		progress = (float) reps / (float) Repetitions;

		base.Update();

		if(reps >= Repetitions)
		{
			TaskCompleted();
		}
	}
}
