using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : UnitySingleton<GameManager>
{
    public GameObject StartMenu;
    public GameObject DeathMenu;
    public GameObject TileManager;
    public Text ScoreText;

    private StartMenu SM;
    private DeathMenu DM;
    private TileManager TM;
    private bool isRunning;
    private bool isDead;
    private float score;
    private float awardSpeed;
    
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        isRunning = false;
        isDead = false;
        SM = StartMenu.GetComponent<StartMenu>();
        DM = DeathMenu.GetComponent<DeathMenu>();
        TM = TileManager.GetComponent<TileManager>();
        score = 0.0f;
        awardSpeed = 0.0f;
    }

    void Update()
    {
        if(!isRunning)
        {
            if ((Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Mouse0)))
            {
                GO();
            }
        }
        else if (!isDead) {
            //// TODO: Your Implementation:
            //// 1. update score (Hint: you can use running time as the score)
            //// 2. show score (Hint: show in Canvas/CurrentScore/Text)
            score += Time.deltaTime * awardSpeed;
            ScoreText.text = "Score： " + (int)score;
        }
        else if (Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Mouse0)) {
            Restart();
        }
    }

    public bool IsRunning() {
        return isRunning && !isDead;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void UpdateSpeed(float speed)
    {
        awardSpeed = speed;
    }

    public void OnDeath(bool collision){
        isDead = true;
        print("GameOver");
        //// TODO: Your Implementation:
        //// 1. show DeathMenu (Hint: you can use Show() in DeathMenu.cs)
        //// 2. stop player
        //// 3. hide all tiles (Hint: call function in TileManager.cs)
        //// 4. record high score (Hint: use PlayerPrefs)
        DM.Show(score);
        TM.hideAll();
    }

    public void Restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GO()
    {
        isRunning = true;
        SM.Hide();
    }

}
