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

	private Transform handleTrans;
	private Image handleImage;

	private float pissError = 0.0f;
	private float pissErrorDelta = 0.0f;
	private float pissCorrection = 0.0f;

	public Color goodPissColor = new Color(1.0f, 0.9f, 0.4f);
	public Color badPissColor = new Color(0.3f, 0.25f, 0.2f);

	private Slider slider;
	private Text text;

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

		sliderGO.transform.position = transform.position + new Vector3(0.6f, -0.5f, 0.0f);

		handleTrans = sliderGO.transform.FindChild("Handle Slide Area").FindChild("Handle");
		if (handleTrans != null)
		{
			handleImage = handleTrans.gameObject.GetComponent<Image>();
			handleImage.color = goodPissColor;
		}
		else
		{
			Debug.LogWarning("no slider handle found");
		}


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

		if (System.Math.Abs(pissError - pissCorrection) < 10) // need to hit with 10deg accuracy
		{
			handleImage.color = goodPissColor;
		}
		else
		{
			handleImage.color = badPissColor;
		}

	}

	private void OnDestroy()
	{
		GameObject.Destroy(sliderGO);
	}

	public void FixedUpdate()
	{
		pissErrorDelta += (Random.value - 0.5f) * 0.2f;
		pissErrorDelta = System.Math.Max(pissErrorDelta, -0.5f);
		pissErrorDelta = System.Math.Min(pissErrorDelta, 0.5f);
		pissError += pissErrorDelta;
		pissError = System.Math.Max(pissError, -50f);
		pissError = System.Math.Min(pissError, 50f);

		if (System.Math.Abs(pissError - pissCorrection) < 10) // need to hit with 10deg accuracy
		{
			repetitionNr++;
			handleImage.color = goodPissColor;
		}
		else
		{
			handleImage.color = badPissColor;
		}
	}

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
