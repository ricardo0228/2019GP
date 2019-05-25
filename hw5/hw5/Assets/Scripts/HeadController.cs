using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadController : MonoBehaviour
{
    public float speed = 0.1f;
    private Vector3 angle = new Vector3(0, 0, 0);
    private Vector3 originPosition;
    private Quaternion originAngle;

    void Start()
    {
        originPosition = transform.position;
        originAngle = transform.rotation;
    }

    void FixedUpdate()
    {
        //WASD控制上下左右
        Vector3 pos = transform.position;
        if (Input.GetKey("w"))
            pos.z += speed;
        if (Input.GetKey("s"))
            pos.z -= speed;
        if (Input.GetKey("a"))
            pos.x -= speed;
        if (Input.GetKey("d"))
            pos.x += speed;
        transform.position = pos;
        //QE控制头部旋转
        if (Input.GetKey("q"))
            angle.y -= speed;
        else if (Input.GetKey("e"))
            angle.y += speed;
        else
            angle = new Vector3(0, 0, 0);
        transform.Rotate(angle);
        //R归位
        if (Input.GetKey("r"))
        {
            transform.position = originPosition;
            transform.rotation = originAngle;
        }
    }
}
