using UnityEngine;
using System.Collections;


[System.Serializable]
public class Day
{
	public GameObject DayBillboard;
	public GameObject[] Tasks;
}

public class TasksResource : ScriptableObject
{
	public Day[] Days;
}
