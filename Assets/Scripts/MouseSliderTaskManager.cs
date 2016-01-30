using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MouseSliderTaskManager : MonoBehaviour
{
	public float LifeTime = 3.0f;

	public int Repetitions = 5;
	private int reps = 0;

	public GameObject SliderPrefab;

	private GameObject sliderGO;
	private Slider slider;
	private Text text;

	private float creationTime;
	private int gridPosition = -1;

	// Use this for initialization
	void Start ()
	{
		creationTime = Time.time;

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

	public void SetGridPosition (int pos)
	{
		gridPosition = pos;
	}

	private void OnDestroy()
	{
		GameObject.Destroy(sliderGO);
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
	}
}
