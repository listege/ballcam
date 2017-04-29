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
		Transform childTransform = transform.FindChild("toungue");
		audioSource = childTransform.GetComponent<AudioSource> ();
		// 모양 설정은 여기서
		//Transform childTransform = transform.FindChild("toungue");
		//MeshRenderer renderer = childTransform.GetComponent<MeshRenderer> ();
		//renderer.material.color = uniqueColors [uniqueIndex];
		audioSource.clip = uniqueLoops[uniqueIndex];
		audioSource.Play ();
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

	public void Activate(bool state)
	{
		isPlaying = state;
		// TEMP
		Transform childTransform = transform.FindChild("toungue");
		if (isPlaying == true)
		{
			Vector3 tounguePos = childTransform.localPosition;
			tounguePos.z = 0.24f;
			childTransform.localPosition = tounguePos;
		}
		else
		{
			Vector3 tounguePos = childTransform.localPosition;
			tounguePos.z = 0.5f;
			childTransform.localPosition = tounguePos;
		}
		// TEMP
		camera.gameObject.SetActive (!state);

		UnityStandardAssets.ImageEffects.NoiseAndGrain noise = camera.gameObject.GetComponent<UnityStandardAssets.ImageEffects.NoiseAndGrain> ();
		noise.intensityMultiplier = 0f;
	}

	public void GameOver()
	{
		isPlaying = false;
		camera.gameObject.SetActive (false);
		audioSource.spatialBlend = 0;
		// TEMP
		Transform childTransform = transform.FindChild("toungue");
		Vector3 tounguePos = childTransform.localPosition;
		tounguePos.z = 0.24f;
		childTransform.localPosition = tounguePos;
		// TEMP
	}

	public float CheckAngle(BallController otherController)
	{
		// 두 오브젝트 위치 벡터
		Vector3 directionVector = (otherController.transform.localPosition - transform.localPosition).normalized;
		// 카메라 전방 벡터
		Vector3 cameraFrontVector = camera.transform.forward;

		return Vector3.Dot (directionVector, cameraFrontVector);
	}

	public void LookAt(Vector3 position)
	{
		transform.LookAt (position);
	}
}
