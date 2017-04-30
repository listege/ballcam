using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMusic : MonoBehaviour
{
    protected int lastScene = -999;
	void Update ()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        if (currentScene != lastScene)
        {
            if (currentScene == 1) // Title
                DontDestroyOnLoad(gameObject);
            if (currentScene >= 3)
                Destroy(gameObject);
            lastScene = SceneManager.GetActiveScene().buildIndex;
        }
	}
}
