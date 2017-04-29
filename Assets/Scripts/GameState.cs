using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
	static public GameState Instance = null;

	public Camera endingCamera = null;
	public Text playingTimeText = null;
	[HideInInspector]
	public AudioSource audioSource = null;
	[HideInInspector]
	public float playingTime = 0;

	public float timer = 5f;
	public float pretimer = 1f;

	[HideInInspector]
	public BallController[] ballControllers = new BallController[2];
	[HideInInspector]
	public int ballCursor = 0;
	[HideInInspector]
	public bool isGameOver = false;

	bool GrainPlayed = false;
	void Awake()
	{
		audioSource = GetComponent<AudioSource> ();
		audioSource.Play ();
	}

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
			if(playingTimeText != null)
				playingTimeText.text = string.Format ("{0:D2}:{1:D2}.{2:D3}", minutes, seconds, milliseconds);

			if (playingTime > (tickCount + 1) * timer - pretimer) {
				StartWarningVisualFeedBack ();
			}

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
			audioSource.volume = Mathf.Max(0, (playingController.CheckAngle (cameraController) - 0.6f) * 2.5f);
			yield return null;
		}
	}

	void ChangeCam()
	{
		ballCursor = (ballCursor + 1) % ballControllers.Length;
		for (int i = 0; i < ballControllers.Length; i++)
			ballControllers [i].Activate (i == ballCursor);

		GrainPlayed = false;
	}

	public void GameOver()
	{
		if (isGameOver == true)
			return;

		foreach (BallController controllers in ballControllers)
			controllers.GameOver ();
		isGameOver = true;
		StartCoroutine ("Coroutine_EndingObject");
		StartCoroutine ("Coroutine_EndingCamera");
	}

	IEnumerator Coroutine_EndingObject()
	{
		ballControllers [0].LookAt (ballControllers[0].transform.localPosition + (ballControllers[0].transform.localPosition - (ballControllers [1].transform.localPosition) - new Vector3(0, 1, 0)));
		ballControllers [1].LookAt (ballControllers[1].transform.localPosition + (ballControllers[1].transform.localPosition - (ballControllers [0].transform.localPosition) - new Vector3(0, 1, 0)));

		yield return null;
	}

	IEnumerator Coroutine_EndingCamera()
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
			if (Input.anyKeyDown)
			{
				int sceneIndex = SceneManager.GetActiveScene ().buildIndex;
				SceneManager.LoadScene (sceneIndex + 1);
			}
				
			yield return null;
		}
	}

	void StartWarningVisualFeedBack(){
		if (!GrainPlayed) {
			
			Camera cam = null;
			foreach (BallController bc in ballControllers){
				if (bc.IsActive ()) {
					cam = bc.GetCamera ();
					break;
				}
			}

			if (!cam)
				return;
			
			StartCoroutine (Coroutine_StartNoiseGrain (cam));
			GrainPlayed = true;
		}
	}

	IEnumerator Coroutine_StartNoiseGrain(Camera cam)
	{
		
		UnityStandardAssets.ImageEffects.NoiseAndGrain noise = cam.gameObject.GetComponent<UnityStandardAssets.ImageEffects.NoiseAndGrain> ();

		float start = noise.intensityMultiplier;
		float starttime = 0f;

		while (starttime < 1f)
		{
			starttime += Time.deltaTime;
			noise.intensityMultiplier = Mathf.Lerp (start, 7.5f, starttime);

			yield return null;
		}
	}
}