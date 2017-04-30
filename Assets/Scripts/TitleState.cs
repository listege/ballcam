using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

	void Update()
	{
		if (Input.anyKeyDown)
		{
			SceneManager.LoadScene ("testscene_lv1");
		}
	}
}
