using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialLevel1 : MonoBehaviour {

	Image img;

	public float TutorialTime = 35f;

	bool IsTutorialLoaded = false;

	// Use this for initialization
	void Start () {
		img = GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!GameState.tutoriallvl1played && !IsTutorialLoaded && Time.timeSinceLevelLoad > TutorialTime) {
			img.color = new Color (1f, 1f, 1f, 1f);
			GameState.tutoriallvl1played = true;
		}
	}
}
