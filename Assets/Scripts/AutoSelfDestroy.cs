using UnityEngine;
using System.Collections;

public class AutoSelfDestroy : MonoBehaviour {

	public float Lifetime = 2.0f;

	private float startTime;

	void Start ()
	{
		startTime = Time.time;
	}
	
	void Update ()
	{
		if(Time.time > startTime + Lifetime)
		{
			GameObject.Destroy(gameObject);
		}
	}
}
