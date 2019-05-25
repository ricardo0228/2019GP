using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//// UI controller for DeathMenu
public class DeathMenu : MonoBehaviour
{
    public Text endScoreText;
    public Text highScoreText;
    public Image bgImage;

    private bool isShowned;
    private float bgTransition;
    private float highScore;

    void Start()
    {
        //// hide the Death Menu at first
        gameObject.SetActive(false);
        isShowned = false;
        bgTransition = 0.0f;
        highScore = PlayerPrefs.GetFloat("HighScore", 0F);
    }
    void Update()
    {
        if(!isShowned)
            return;
        
        //// fading effect
        bgTransition += Time.deltaTime * 2;
        bgImage.color = Color.Lerp(new Color(0,0,0,0), Color.black, bgTransition);
    }

    public void Show(float score){
        //// TODO: Your Implementation:
        //// - show Death Menu
        //// - show score information
        if (score > highScore) highScore = score;
        PlayerPrefs.SetFloat("HighScore", highScore);
        gameObject.SetActive(true);
        isShowned = true;
        endScoreText.text = "Score： " + (int)score;
        highScoreText.text = "High Score： " + (int)highScore;
    }
}
