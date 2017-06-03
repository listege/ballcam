using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialArrowKeys : MonoBehaviour
{
	protected static bool IsDisplayed = false;
	Image img;

	public float TutorialTime = 3f;

	bool IsTutorialLoaded = false;

	GameState gs;

	// Use this for initialization
	void Start ()
	{
		if (SceneManager.GetActiveScene ().name != "testscene_lv1" || IsDisplayed == true)
		{
			gameObject.SetActive (false);
			return;
		}

		img = GetComponent<Image> ();
		GameObject go = GameObject.Find ("gamestate");
		gs = null;
		if (go)
			gs = go.GetComponent<GameState> ();
	}

	// Update is called once per frame
	void Update () {
		if (gs && !gs.isGameOver && !IsTutorialLoaded && Time.timeSinceLevelLoad > TutorialTime) {
			img.color = new Color (1f, 1f, 1f, 1f);
			IsDisplayed = true;
			IsTutorialLoaded = true;
		}

		if (IsTutorialLoaded && Time.timeSinceLevelLoad > TutorialTime + 5f) {
			img.color = new Color (1f, 1f, 1f, 0f);
		}
	}
}
