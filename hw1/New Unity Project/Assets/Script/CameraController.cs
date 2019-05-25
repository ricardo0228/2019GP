using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject ball;

    private Vector3 offset;

    void Start()
    {
        offset = transform.position - ball.transform.position;
    }

    void LateUpdate()
    {
        float k = 1 - ball.transform.position.y / offset.y;
        transform.position = ball.transform.position + offset * k;
    }
}
