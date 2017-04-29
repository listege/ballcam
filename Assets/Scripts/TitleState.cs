using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleState : MonoBehaviour
{
	public GameObject cubePrefab = null;

	void Start ()
	{
		for (int row = -3; row <= 3; row++)
		{
			for (int col = -6; col <= 6; col++)
			{
				GameObject newCube = Instantiate (cubePrefab) as GameObject;
				newCube.transform.localPosition = new Vector3 (col * 1.5f, row * 1.5f, 0);
			}
		}
	}
}
