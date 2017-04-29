using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
	static public GameState Instance = null;

	public Camera endingCamera = null;
	public Text playingTimeText = null;
	[HideInInspector]
	public float playingTime = 0;

	public float timer = 5f;
	[HideInInspector]
	public BallController[] ballControllers = new BallController[2];
	[HideInInspector]
	public int ballCursor = 0;
	[HideInInspector]
	public bool isGameOver = false;

	// Use this for initialization
	void Start ()
	{
		Instance = this;

		BallController[] foundControllers = FindObjectsOfType<BallController> ();
		foreach (BallController foundController in foundControllers) {
			ballControllers [foundController.uniqueIndex] = foundController;
			foundController.Activate (foundController.uniqueIndex == 0);
		}

		StartCoroutine ("Coroutine_ChangeCam");
		//InvokeRepeating ("ChangeCam", timer, timer);
	}

	IEnumerator Coroutine_ChangeCam()
	{
		int tickCount = 0; // 혹시 몰라서 저장
		while (true)
		{
			if (isGameOver == true)
				break;
			
			playingTime += Time.deltaTime;
			int minutes = (int)(playingTime / 60);
			int seconds = (int)(playingTime - minutes * 60);
			int milliseconds = (int)((playingTime - minutes * 60 - seconds) * 1000);
			playingTimeText.text = string.Format ("{0:D2}:{1:D2}.{2:D3}", minutes, seconds, milliseconds);

			if (playingTime > (tickCount + 1) * timer)
			{
				ChangeCam ();
				tickCount++;
			}

			// Angle 검사
			BallController playingController = null;
			BallController cameraController = null;
			foreach(BallController controller in ballControllers)
			{
				if (controller.isPlaying)
					playingController = controller;
				else
					cameraController = controller;
			}
			// 대략 0.8 이상 정도면 보인다고 생각하면 될 듯
			Debug.Log(playingController.CheckAngle (cameraController));
			yield return null;
		}
	}

	void ChangeCam()
	{
		ballCursor = (ballCursor + 1) % ballControllers.Length;
		for (int i = 0; i < ballControllers.Length; i++)
			ballControllers [i].Activate (i == ballCursor);
	}

	public void GameOver()
	{
		if (isGameOver == true)
			return;

		foreach (BallController controllers in ballControllers)
			controllers.GameOver ();
		isGameOver = true;
		StartCoroutine ("Coroutine_Ending");
	}

	IEnumerator Coroutine_Ending()
	{
		const float cameraRotationSpeed = 60f;
		Vector3 cameraPosition = Vector3.zero;
		foreach (BallController controller in ballControllers)
			cameraPosition += controller.transform.localPosition;
		cameraPosition /= ballControllers.Length;
		cameraPosition.y = 10;
		endingCamera.transform.localPosition = cameraPosition;
		endingCamera.gameObject.SetActive (true);

		float rotation = 0;
		while (true)
		{
			rotation += cameraRotationSpeed * Time.deltaTime;
			endingCamera.transform.localRotation = Quaternion.Euler(90, rotation, 0);
			yield return null;
		}
	}
}