using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {

	public float timer = 5f;

	public Camera Camera1;
	public Camera Camera2;
	public BallController b1;
	public BallController b2;

	// Use this for initialization
	void Start () {
		InvokeRepeating ("ChangeCam", timer, timer);
	}

	void ChangeCam(){
		if (Camera1.gameObject.activeSelf) {
			Camera2.gameObject.SetActive (true);
			Camera1.gameObject.SetActive (false);
			b2.isPlaying = false;
			b1.isPlaying = true;
		} else if (Camera2.gameObject.activeSelf) {
			Camera1.gameObject.SetActive (true);
			Camera2.gameObject.SetActive (false);
			b2.isPlaying = true;
			b1.isPlaying = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
