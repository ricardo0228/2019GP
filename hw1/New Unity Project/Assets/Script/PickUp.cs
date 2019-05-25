using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public float speed;
    private float totalTime = 0;

    void Update()
    {
        transform.Rotate(new Vector3(0, 45, 0) * Time.deltaTime * speed);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f) + 
            new Vector3(0.2f, 0.2f, 0.2f) * Mathf.Sin(totalTime) * speed;
        totalTime += Time.deltaTime;
    }
}
