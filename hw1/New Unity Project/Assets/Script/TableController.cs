using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableController : MonoBehaviour
{
    public float speed;
    public float limit;
    private Vector3 angle;

    void Start()
    {
        angle = new Vector3(0, 0, 0);
    }

    void FixedUpdate()
    {
        transform.RotateAround(new Vector3(0f, 0f, 0f), new Vector3(1f, 0f, 0f), -angle.z);
        transform.RotateAround(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 1f), -angle.x);

        float moveHorizontal = -Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        
        angle += new Vector3(moveHorizontal, 0.0f, moveVertical) * speed;
        if (angle.x > limit) angle.x = limit;
        if (angle.x < -limit) angle.x = -limit;
        if (angle.z > limit) angle.z = limit;
        if (angle.z < -limit) angle.z = -limit;

        //transform.Rotate(angle);
        transform.RotateAround(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 1f), angle.x);
        transform.RotateAround(new Vector3(0f, 0f, 0f), new Vector3(1f, 0f, 0f), angle.z);
    }
}
