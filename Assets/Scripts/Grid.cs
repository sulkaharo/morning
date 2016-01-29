using UnityEngine;
using System.Collections;

public class GridDefinition
{
	public static int Size = 9;

	public bool[] Occupied;
	public GameObject[] TaskInPosition;

	public void Grid()
	{
		Occupied = new bool[Size];
		TaskInPosition = new GameObject[Size];
		Debug.Log("Grid initialized, I think");
		for (int i = 0; i < Size; i++ )
		{
			TaskInPosition[i] = null;
		}
	}
	
	//index of next free grid position
	public int GetNextEmpty()
	{
		int firstFree = -1;

		for (int i = 0; i < Size; i++ )
		{
			if(TaskInPosition[i] == null)
			{
				firstFree = i;
				break;
			}
		}

		return firstFree;
	}
}
