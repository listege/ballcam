﻿using System.Collections;
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
	
	// Update is called once per frame
	void Update () {

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
		if (collision.gameObject.layer == 8)
			_stickyCount++;
		else if (collision.gameObject.layer == 9)
			_slipperyCount++;

		RecalcDrag ();
	}

	void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.layer == 8)
			_stickyCount--;
		else if (collision.gameObject.layer == 9)
			_slipperyCount--;

		RecalcDrag ();
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
}
