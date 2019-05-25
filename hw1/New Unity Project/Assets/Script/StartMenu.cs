using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public Button btnPlay;
    public Button btnExit;

    void Start()
    {
        btnPlay.onClick.AddListener(Play);
        btnExit.onClick.AddListener(Exit);
    }

    void Play()
    {
        SceneManager.LoadScene("GameScene");
    }

    void Exit()
    {
        Application.Quit();
    }
}
