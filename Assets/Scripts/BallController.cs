using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

	public bool isPlaying = false;

	public int uniqueIndex = 0;
	public Color[] uniqueColors = null;
	public float clamp = 0.1f;
	public float amplitude = 0.3f;

	Rigidbody rbd;
	Camera camera;
	float x;
	float y;

	void Awake ()
	{
		rbd = GetComponent<Rigidbody> ();
		Transform cameraTransform = transform.FindChild ("Camera_1");
		camera = cameraTransform.GetComponent<Camera> ();

		// 모양 설정은 여기서
		Transform childTransform = transform.FindChild("Sphere");
		MeshRenderer renderer = childTransform.GetComponent<MeshRenderer> ();
		renderer.material.color = uniqueColors [uniqueIndex];
	}
	
	void FixedUpdate ()
	{
		if (!isPlaying)
			return;

		Vector3 power = new Vector3(0f, 0f, 0f);

		if (Input.GetKey (KeyCode.UpArrow)) {
			power.x =  amplitude;
		} else if (Input.GetKey (KeyCode.DownArrow)) {
			power.x = -amplitude;
		} else if (Input.GetKey (KeyCode.LeftArrow)) {
			power.z = amplitude;	
		} else if (Input.GetKey (KeyCode.RightArrow)) {
			power.z = -amplitude;
		}

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

	public void Activate(bool state)
	{
		isPlaying = state;
		camera.gameObject.SetActive (!state);
	}

	public void GameOver()
	{
		isPlaying = false;
		camera.gameObject.SetActive (false);
	}

	public float CheckAngle(BallController otherController)
	{
		// 두 오브젝트 위치 벡터
		Vector3 directionVector = (otherController.transform.localPosition - transform.localPosition).normalized;
		// 카메라 전방 벡터
		Vector3 cameraFrontVector = camera.transform.forward;

		return Vector3.Dot (directionVector, cameraFrontVector);
	}
}
