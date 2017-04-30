using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialLevel1 : MonoBehaviour {

	Image img;

	public float TutorialTime = 35f;

	bool IsTutorialLoaded = false;

	GameState gs;

	// Use this for initialization
	void Start () {
		img = GetComponent<Image> ();
		GameObject go = GameObject.Find ("gamestate");
		gs = null;
		if (go)
			gs = go.GetComponent<GameState> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (gs && !gs.isGameOver && !GameState.tutoriallvl1played && !IsTutorialLoaded && Time.timeSinceLevelLoad > TutorialTime) {
			img.color = new Color (1f, 1f, 1f, 1f);
			GameState.tutoriallvl1played = true;
			IsTutorialLoaded = true;
		}

		if (IsTutorialLoaded && Time.timeSinceLevelLoad > TutorialTime + 5f) {
			img.color = new Color (1f, 1f, 1f, 0f);
		}
	}
}
