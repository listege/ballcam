using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAnimator : MonoBehaviour {

	RectTransform rect;

	public float Speed = 0.1f;

	bool ready = false;


	// Use this for initialization
	void Start () {
		rect = GetComponent<RectTransform> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = rect.transform.position; 
		pos.y += Speed;

		rect.position = pos;

		if (!ready && rect.position.y > 675f) {
			ready = true;
		}

		if (ready && Input.anyKeyDown) {

			Application.LoadLevel (Application.loadedLevel + 1);
			
		}
	}
}
