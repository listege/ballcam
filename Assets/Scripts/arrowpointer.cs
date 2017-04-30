using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowpointer : MonoBehaviour {

	public Vector3 offset;

	public float offsetmult;

	GameState gamestate;

	// Use this for initialization
	void Start () {
		gamestate = GameObject.Find ("gamestate").GetComponent<GameState> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		Camera cam = gamestate.GetCurrentCamera ();
		Debug.Log (cam.gameObject.transform.parent.name);
		Debug.Log (cam.transform.forward);
		if (cam)
			transform.position = cam.transform.position + cam.transform.forward * offsetmult + offset;

		Vector3 power = new Vector3(0f, 0f, 0f);

		power.x = Input.GetAxis ("Vertical");
		power.z = -Input.GetAxis ("Horizontal");
		Quaternion q = Quaternion.LookRotation(power, Vector3.up);
		q = Quaternion.Euler( q.eulerAngles + new Vector3 (0f, 90f, 0f));
		transform.rotation = q;


	}



}
