using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{

    public Text startText;
    
    private Color baseColor;
    private float circleTime = 1.0f;

    void Start()
    {
        gameObject.SetActive(true);
        baseColor = startText.color;
    }
    void Update()
    {
        float lerp = Mathf.PingPong(Time.time, circleTime) / circleTime + 0.2f;
        startText.color = Color.Lerp(new Color(baseColor.r, baseColor.b, baseColor.g, 0), baseColor, lerp);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
