using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {

	public float timer = 5f;

	public BallController[] ballControllers = new BallController[2];
	public int ballCursor = 0;

	// Use this for initialization
	void Start ()
	{
		BallController[] foundControllers = FindObjectsOfType<BallController> ();
		foreach (BallController foundController in foundControllers)
		{
			ballControllers [foundController.uniqueIndex] = foundController;
			foundController.Activate (foundController.uniqueIndex == 0);
		}

		InvokeRepeating ("ChangeCam", timer, timer);
	}

	void ChangeCam()
	{
		ballCursor = (ballCursor + 1) % ballControllers.Length;
		for (int i = 0; i < ballControllers.Length; i++)
			ballControllers [i].Activate (i == ballCursor);
	}
	
	// Update is called once per frame
	void Update ()
	{
		// 게임 클리어 검사
		//if (Vector3.Distance (b1.transform.localPosition, b2.transform.localPosition) <= 1.0f)
		//{
		//	Debug.Log ("MEEET");
		//}
	}
}