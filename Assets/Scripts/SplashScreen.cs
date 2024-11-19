using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{

    private float timer = 0f;
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > 5)
        {
            VideoFinished();
        }
    }

    private void VideoFinished()
    {
        SceneManager.LoadScene("MainMenusScene");
    }
}
