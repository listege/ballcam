using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

	public bool isPlaying = false;

	public int uniqueIndex = 0;
	public Color[] uniqueColors = null;
	public AudioClip[] uniqueLoops = null;
	public float clamp = 0.1f;
	public float amplitude = 0.3f;

	public GameObject Tongue;

	AudioSource audioSource = null;
	Rigidbody rbd;
	GameState gamestate;
	Camera camera;
	float x;
	float y;

	void Awake ()
	{
		gamestate = GameObject.Find ("gamestate").GetComponent<GameState>();
		rbd = GetComponent<Rigidbody> ();
		Transform cameraTransform = transform.FindChild ("Camera_1");
		camera = cameraTransform.GetComponent<Camera> ();
		//Transform childTransform = transform.FindChild("toungue");
		//audioSource = childTransform.GetComponent<AudioSource> ();
		audioSource = GetComponent<AudioSource> ();
		// 모양 설정은 여기서
		//Transform childTransform = transform.FindChild("toungue");
		//childTransform.gameObject.layer = 11 + uniqueIndex;
	}
	
	void FixedUpdate ()
	{
		if (!isPlaying)
			return;

		Vector3 power = new Vector3(0f, 0f, 0f);

		/*
		if (Input.GetKey (KeyCode.UpArrow)) {
			power.x =  amplitude;
		} else if (Input.GetKey (KeyCode.DownArrow)) {
			power.x = -amplitude;
		} else if (Input.GetKey (KeyCode.LeftArrow)) {
			power.z = amplitude;	
		} else if (Input.GetKey (KeyCode.RightArrow)) {
			power.z = -amplitude;
		}
		*/


		power.x = Input.GetAxis ("Vertical") * amplitude;
		power.z = -Input.GetAxis ("Horizontal") * amplitude;

		rbd.AddForce (power);
	}

	protected int _stickyCount = 0;
	protected int _slipperyCount = 0;
	void OnCollisionEnter(Collision collision)
	{
		switch (collision.gameObject.layer)
		{
		case 8:
			_stickyCount++;
			RecalcDrag ();
			break;
		case 9:
			_slipperyCount++;
			RecalcDrag ();
			break;
		case 10:
			GameState.Instance.GameOver ();
			break;
		}
	}

	void OnCollisionExit(Collision collision)
	{
		switch (collision.gameObject.layer)
		{
		case 8:
			_stickyCount--;
			RecalcDrag ();
			break;
		case 9:
			_slipperyCount--;
			RecalcDrag ();
			break;
		}
	}

	protected void RecalcDrag()
	{
		if (_stickyCount > 0)
			rbd.drag = 6;
		else if (_slipperyCount > 0)
			rbd.drag = 0.5f;
		else
			rbd.drag = 2;
	}

	public Camera GetCamera(){
		return camera;
	}

	public bool IsActive()
	{
		return camera.gameObject.activeSelf;
	}

	public void Activate(bool state, bool forceTurnOffCamera = false)
	{
		isPlaying = state;
		// TEMP
		Transform childTransform = transform.FindChild("toungue");
		if (isPlaying == true || forceTurnOffCamera == true)
			childTransform.gameObject.SetActive (true);
		else
			childTransform.gameObject.SetActive (false);
		// TEMP
		if(forceTurnOffCamera == true)
			camera.gameObject.SetActive (false);
		else
			camera.gameObject.SetActive (!state);

		UnityStandardAssets.ImageEffects.NoiseAndGrain noise = camera.gameObject.GetComponent<UnityStandardAssets.ImageEffects.NoiseAndGrain> ();
		noise.intensityMultiplier = 0f;
	}

	public void GameStart()
	{
		audioSource.clip = uniqueLoops[GameState.Instance.musicSetIndex * 2 + uniqueIndex];
		audioSource.Play ();
	}

	public void GameOver()
	{
		isPlaying = false;
		rbd.isKinematic = true;
		camera.gameObject.SetActive (false);
		audioSource.spatialBlend = 0;
		// TEMP
		Transform childTransform = transform.FindChild("toungue");
		childTransform.gameObject.SetActive (true);
		StartCoroutine ("Coroutine_ToungueRoot");
		StartCoroutine ("Corotuine_ToungueDance");
		// TEMP
	}

	IEnumerator Coroutine_ToungueRoot()
	{
		Transform childTransform = transform.FindChild("toungue");
		Vector3 startAngle = new Vector3(0, 0.42f, 0);
		while (true)
		{
			Vector3 targetAngle = new Vector3(Random.Range (-10f, 10f), 0.42f, Random.Range (-6f, 6f));
			float distance = Vector2.Distance (startAngle, targetAngle);
			while (distance < 7.0f)
			{
				targetAngle = new Vector3(Random.Range (-10f, 10f), 0.42f, Random.Range (-6f, 6f));
				distance = Vector2.Distance (startAngle, targetAngle);
			}

			float estimateTime = (distance / 60.0f) * Random.Range(0.8f, 1.1f);
			for (float timer = 0; timer <= estimateTime; timer += Time.deltaTime)
			{
				float progress = Mathf.Clamp01(timer / estimateTime);
				Vector3 newRotation = startAngle * (1 - progress) + targetAngle * progress;
				childTransform.localRotation = Quaternion.Euler(newRotation.x, 0.42f, newRotation.z);
				yield return null;
			}

			startAngle = targetAngle;
			yield return new WaitForSeconds (Random.Range (0.1f, 0.15f));
		}
	}

	IEnumerator Corotuine_ToungueDance()
	{
		Transform childTransform = transform.FindChild("toungue");
		ChangeLayerRecursively (childTransform, 12);
		childTransform = childTransform.FindChild ("Armature");
		childTransform = childTransform.FindChild ("Bone.001");
		Rigidbody body1 = childTransform.GetComponent<Rigidbody> ();
		childTransform = childTransform.parent.FindChild ("Bone.005");
		Rigidbody body2 = childTransform.GetComponent<Rigidbody> ();
		float maxSpeed = 0;
		while (true)
		{
			{
				Vector3 forceDirection = Random.onUnitSphere;
				body1.AddForce (forceDirection * 500);
				if (body1.velocity.magnitude > 2f)
					body1.velocity = body1.velocity.normalized * 2f;
			}
			yield return new WaitForSeconds (Random.Range (0.2f, 0.3f));

			{
				Vector3 forceDirection = Random.onUnitSphere;
				body2.AddForce (forceDirection * 500);
				if (body2.velocity.magnitude > 2f)
					body2.velocity = body2.velocity.normalized * 2f;
			}

			yield return new WaitForSeconds (Random.Range (0.4f, 0.55f));
		}
	}

	public float CheckAngle(BallController otherController)
	{
		// 두 오브젝트 위치 벡터
		Vector3 directionVector = (otherController.transform.localPosition - transform.localPosition).normalized;
		// 카메라 전방 벡터
		Vector3 cameraFrontVector = camera.transform.forward;

		return Vector3.Dot (directionVector, cameraFrontVector);
	}

	public void LookAt(Vector3 lookPosition, Vector3 moveDirection)
	{
		transform.LookAt (lookPosition);
		Vector3 newPositon = transform.localPosition;
		newPositon += moveDirection * 0.2f;
		transform.localPosition = newPositon;
	}

	protected void ChangeLayerRecursively(Transform targetTransform, int layer)
	{
		targetTransform.gameObject.layer = layer;
		foreach (Transform child in targetTransform)
		{
			ChangeLayerRecursively(child, layer);
		}
	}
}
