using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTrigger : MonoBehaviour
{
    public Text countText;
    public Text gameOverText;

    private int count;
    private float totalTime;

    private void Start()
    {
        count = 0;
        gameOverText.text = "";
        SetText();

    }

    void FixedUpdate()
    {
        totalTime += Time.deltaTime;
        if (transform.position.y < -20)
            SetText();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetText();
        }
    }

    void SetText()
    {
        countText.text = "已收集:  " + count.ToString() + "/10";
        if (count >= 10)
        {
            gameOverText.text = "胜 利！\n用时： " + (int)totalTime + "秒";
        }
        if (transform.position.y < -20)
        {
            gameOverText.text = "失 败！";
        }
    }
}
