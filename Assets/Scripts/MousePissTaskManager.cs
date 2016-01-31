using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MousePissTaskManager : TaskManagerBase
{
	public GameObject SliderPrefab;
	public GameObject PissPrefab;

	private GameObject sliderGO;
	private GameObject pissGO;
	private Transform pissT;

	private float pissError = 0.0f;
	private float pissErrorDelta = 0.0f;
	private float pissCorrection = 0.0f;

	private Slider slider;
	private Text text;

	// Use this for initialization
	public override void Start ()
	{
		base.Start();


		pissGO = GameObject.Instantiate(PissPrefab);
		pissT = pissGO.transform;
		pissT.SetParent(gameObject.transform, false);
		pissT.localPosition = new Vector3(0.45f, 0.0f, 0.0f);

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
		slider.value = 0.5f;
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
			text.text = (TotalReps - repetitionNr).ToString();
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
			repetitionNr++;
		}
		pissCorrection = -50.0f + 100.0f * val;
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

	public void FixedUpdate()
	{
		pissErrorDelta += (Random.value - 0.5f) * 0.2f;
		pissErrorDelta = System.Math.Max(pissErrorDelta, -0.5f);
		pissErrorDelta = System.Math.Min(pissErrorDelta, 0.5f);
		pissError += pissErrorDelta;
		pissError = System.Math.Max(pissError, -50f);
		pissError = System.Math.Min(pissError, 50f);

		if (System.Math.Abs(pissError - pissCorrection) < 10)
		{
			repetitionNr++;
		}

	}

	// Update is called once per frame
	public override void Update ()
	{
		progress = (float) repetitionNr / (float) TotalReps;

		pissT.localEulerAngles = new Vector3(0.0f, 0.0f, pissError - pissCorrection);

		base.Update();

		if(repetitionNr >= TotalReps)
		{
			TaskCompleted();
		}
	}
}
