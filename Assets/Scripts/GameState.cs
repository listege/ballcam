using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
	static public GameState Instance = null;

	[HideInInspector]
	public Camera endingCamera = null;
	[HideInInspector]
	public Text playingTimeText = null;
	[HideInInspector]
	public Text bestTimeText = null;
	[HideInInspector]
	public Image resetTimerImage = null;
	[HideInInspector]
	public Image endingImage = null;
	[HideInInspector]
	public AudioSource audioSource = null;
	[HideInInspector]
	public float playingTime = 0;

	public float timer = 5f;
	public float pretimer = 1f;
	protected float resetTimer = 0;

	public arrowpointer arrowhelper;

	[HideInInspector]
	public BallController[] ballControllers = new BallController[2];
	[HideInInspector]
	public int ballCursor = 0;
	[HideInInspector]
	public bool isGameOver = false;
	[HideInInspector]
	public int musicSetIndex = 0;

	bool GrainPlayed = false;
	void Awake()
	{
		audioSource = GetComponent<AudioSource> ();
		musicSetIndex = Random.Range (0, 2);
	}

	// Use this for initialization
	void Start ()
	{
		Instance = this;

		GameObject[] rootObjects = SceneManager.GetActiveScene ().GetRootGameObjects ();
		foreach (GameObject rootObject in rootObjects)
		{
			switch (rootObject.name)
			{
			case "GlobalCamera":
				endingCamera = rootObject.GetComponent<Camera>();
				break;
			case "Canvas":
				playingTimeText = rootObject.transform.FindChild ("TimeText").GetComponent<Text> ();
				playingTimeText.text = string.Format ("Stage {0}", SceneManager.GetActiveScene ().buildIndex);
				resetTimerImage = rootObject.transform.FindChild ("ResetImage").GetComponent<Image> ();
				bestTimeText = rootObject.transform.FindChild ("BestText").GetComponent<Text> ();
				if (PlayerPrefs.HasKey (SceneManager.GetActiveScene ().name)) {
					float bestTime = PlayerPrefs.GetFloat (SceneManager.GetActiveScene ().name);
					int minutes = (int)(bestTime / 60);
					int seconds = (int)(bestTime - minutes * 60);
					bestTimeText.text = string.Format ("BEST {0:D2}:{1:D2}", minutes, seconds);
				}
				endingImage = rootObject.transform.FindChild ("ClearImage").GetComponent<Image> ();
				break;
			}
		}

		BallController[] foundControllers = FindObjectsOfType<BallController> ();
		foreach (BallController foundController in foundControllers) {
			ballControllers [foundController.uniqueIndex] = foundController;
			foundController.Activate (false, true);
		}

		Instantiate (arrowhelper);

		StartCoroutine ("Coroutine_Overview");
		//InvokeRepeating ("ChangeCam", timer, timer);
	}

	void Update(){
		
		// !!!!! 짧은 게임 이 기 에 performance 걱정없이 고!
		BallController playingController = null;
		BallController cameraController = null;
		foreach(BallController controller in ballControllers)
		{
			if (controller.isPlaying)
				playingController = controller;
			else
				cameraController = controller;
		}

		if (playingController && cameraController) {

			//Debug.Log ("playingController");

			float clr_r = Mathf.Lerp (1f, 0f, Mathf.InverseLerp (0.6f, 1f, playingController.CheckAngle (cameraController)));
			float clr_g = Mathf.Lerp (0f, 1f, Mathf.InverseLerp (0.6f, 1f, playingController.CheckAngle (cameraController)));
			float clr_b = Mathf.Lerp (0.2f, 0.2f, Mathf.InverseLerp (0.6f, 1f, playingController.CheckAngle (cameraController)));

			Debug.Log (Mathf.InverseLerp (-1f, 1f, playingController.CheckAngle (cameraController)));

			playingController.Lens.material.SetColor ("_Color", new Color (clr_r, clr_g, clr_b));
			playingController.Lens.material.SetColor ("_EmissionColor", new Color (clr_r, clr_g, clr_b));
		}

	}

	IEnumerator Coroutine_Overview()
	{
		endingCamera.transform.localPosition = new Vector3 (0, 25, -6);
		endingCamera.transform.localRotation = Quaternion.Euler (70, 0, 0);
		endingCamera.gameObject.SetActive (true);

		yield return new WaitForSeconds (4.0f);
		audioSource.Play ();
		endingCamera.gameObject.SetActive (false);
		BallController[] foundControllers = FindObjectsOfType<BallController> ();
		foreach (BallController foundController in foundControllers) {
			ballControllers [foundController.uniqueIndex] = foundController;
			foundController.Activate (foundController.uniqueIndex == 0);
			foundController.GameStart ();
		}

		StartCoroutine ("Coroutine_ChangeCam");
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

			if (Input.GetKey (KeyCode.Space) == true) {
				resetTimer += Time.deltaTime;
				if (resetTimer >= 1.5f)
					SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
			} else
				resetTimer = 0;

			if (resetTimer > 0)
			{
				resetTimerImage.gameObject.SetActive (true);
				resetTimerImage.rectTransform.localScale = new Vector3 (Mathf.Clamp01(resetTimer / 1.5f), 1, 1);
			}
			else
			{
				resetTimerImage.gameObject.SetActive (false);
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
		for (int i = 0; i < ballControllers.Length; i++) {
			ballControllers [i].Activate (i == ballCursor);
		}

		/* hardcoded tongue fix */
		/*
		if (ballControllers [0].IsActive ()) {
			ballControllers [0].Tongue.SetActive (false);
			ballControllers [1].Tongue.SetActive (true);
		} else {
			ballControllers [1].Tongue.SetActive (false);
			ballControllers [0].Tongue.SetActive (true);
		}*/

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
		float lastBest = PlayerPrefs.GetFloat (SceneManager.GetActiveScene ().name, -1);
		if (lastBest < 0 || playingTime < lastBest)
		{
			PlayerPrefs.SetFloat (SceneManager.GetActiveScene ().name, playingTime);
			int minutes = (int)(playingTime / 60);
			int seconds = (int)(playingTime - minutes * 60);
			bestTimeText.text = string.Format("BEST {0:D2}:{1:D2}", minutes, seconds);
		}
		endingImage.gameObject.SetActive (true);
	}

	IEnumerator Coroutine_EndingObject()
	{
		Vector3 direction = (ballControllers [0].transform.localPosition - ballControllers [1].transform.localPosition).normalized;
		ballControllers [0].LookAt (
			ballControllers[0].transform.localPosition + (ballControllers[0].transform.localPosition - (ballControllers [1].transform.localPosition) - new Vector3(0, 1, 0)),
			direction
		);
		ballControllers [1].LookAt (
			ballControllers[1].transform.localPosition + (ballControllers[1].transform.localPosition - (ballControllers [0].transform.localPosition) - new Vector3(0, 1, 0)),
			-direction
		);

		yield return new WaitForSeconds (1.5f);
		while (true)
		{
			if (Input.anyKeyDown)
			{
				int sceneIndex = SceneManager.GetActiveScene ().buildIndex;
				SceneManager.LoadScene (sceneIndex + 1);
			}
			yield return null;
		}
	}

	IEnumerator Coroutine_EndingCamera()
	{
		const float cameraRotationSpeed = 60f;
		Vector3 cameraPosition = Vector3.zero;
		foreach (BallController controller in ballControllers)
			cameraPosition += controller.transform.localPosition;
		cameraPosition /= ballControllers.Length;
		cameraPosition.y = 7;
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

	public Camera GetCurrentCamera(){
		foreach (BallController bc in ballControllers) {
			if (bc && !bc.isPlaying) {
				Camera cam = bc.GetCamera ();
				return cam;
			}
		}
		return null;
	}
}