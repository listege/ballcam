using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogoScene : MonoBehaviour
{
	[SerializeField]
	Image _fadeImage = null;

	// Use this for initialization
	void Start () {
		StartCoroutine ("Coroutine_Fade");
	}

	IEnumerator Coroutine_Fade()
	{
		float time = 1;
		for (float timer = 0; timer < time; timer += Time.deltaTime) {
			float progress = Mathf.Clamp01(timer / time);
			Color color = _fadeImage.color;
			color.a = 1 - progress;
			_fadeImage.color = color;
			yield return null;
		}

		yield return new WaitForSeconds(2.5f);

		for (float timer = 0; timer < time; timer += Time.deltaTime) {
			float progress = Mathf.Clamp01(timer / time);
			Color color = _fadeImage.color;
			color.a = progress;
			_fadeImage.color = color;
			yield return null;
		}
		yield return new WaitForSeconds (0.2f);
		SceneManager.LoadScene (1);
	}
}
