using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyController : MonoBehaviour
{
    public void Update()
    {
        KeyControl();
    }

    void KeyControl()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Return();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    void Return()
    {
        SceneManager.LoadScene("StartScene");
    }

    void Restart()
    {
        SceneManager.LoadScene("GameScene");
    }
}
