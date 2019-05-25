using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SimplePendulum : MonoBehaviour
{
    public enum MovementType
    {
        Explicit_Euler, Midpoint, Trapezoid
    }
    public MovementType movementType;
    public Vector3 origin;
    public float gravity = 10f;
    public float len = 4;
    public float startAngle = 30;
    //private float delta = 0.016f;
    private float currentOmega;
    private float currentAngle;
    void Awake()
    {
        currentAngle = startAngle * Mathf.PI / 180;
        currentOmega = 0;
        transform.position = new Vector3(0, -len, origin.z);
        transform.RotateAround(origin, new Vector3(0, 0, 1), startAngle);
    }

    void FixedUpdate()
    {
        float delta = Time.deltaTime;
        float k1, k2, k3;
        float l1, l2, l3;

        //Explicit Euler
        k1 = currentOmega;
        l1 = -(gravity / len) * Mathf.Sin(currentAngle);
        //Midpoint
        k2 = currentOmega + delta * l1 / 2;
        l2 = -(gravity / len) * Mathf.Sin(currentAngle + delta * k1 / 2);
        //Trapezoid
        k3 = currentOmega + delta * l1 / 2;
        l3 = (-(gravity / len) * Mathf.Sin(currentAngle + delta * k1) + l1) / 2;

        if (movementType == MovementType.Explicit_Euler)
        {
            currentAngle += delta * k1;
            currentOmega += delta * l1;
        }
        else if (movementType == MovementType.Midpoint)
        {
            currentAngle += delta * k2;
            currentOmega += delta * l2;
        }
        else if (movementType == MovementType.Trapezoid)
        {
            currentAngle += delta * k3;
            currentOmega += delta * l3;
        }

        transform.position = new Vector3(0, -len, origin.z);
        transform.RotateAround(origin, new Vector3(0, 0, 1), currentAngle * 180 / Mathf.PI);

        //float E = currentOmega * currentOmega * len * len / 2 + gravity * len * (1 - Mathf.Cos(currentAngle));
        //WriteMessage(E.ToString());
    }

    public void WriteMessage(string msg)
    {
        string fileName = "E:/Study/2018-2/游戏设计与开发/hw5/test.txt";

        if (movementType == MovementType.Explicit_Euler)
            fileName = "E:/Study/2018-2/游戏设计与开发/hw5/Explicit_Euler.txt";
        else if (movementType == MovementType.Midpoint)
            fileName = "E:/Study/2018-2/游戏设计与开发/hw5/Midpoint.txt";
        else if (movementType == MovementType.Trapezoid)
            fileName = "E:/Study/2018-2/游戏设计与开发/hw5/Trapezoid.txt";

        using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.BaseStream.Seek(0, SeekOrigin.End);
                sw.WriteLine(msg);
                sw.Flush();
            }
        }
    }
}
