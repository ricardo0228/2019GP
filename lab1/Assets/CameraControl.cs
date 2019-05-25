using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Matrix4x4 originalProjection;
    public void Awake()
    {
        originalProjection = Camera.main.projectionMatrix;
        print(Camera.main.projectionMatrix);
    }
    void Update()
    {
        //改变原始矩阵的某些值
        Matrix4x4 p = originalProjection;
        Camera.main.projectionMatrix = p * Matrix4x4.Scale(new Vector3(1f, 0.5f, 1f));
    }
}
