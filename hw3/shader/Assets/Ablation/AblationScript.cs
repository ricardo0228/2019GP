using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AblationScript : MonoBehaviour
{
    void Start()
    {
        Material mat = GetComponent<Renderer>().material;
        float minY, maxY;
        CalculateMinMaxZ(out minY, out maxY);
        mat.SetFloat("_MinBorderZ", minY);
        mat.SetFloat("_MaxBorderZ", maxY);
    }

    void CalculateMinMaxZ(out float minY, out float maxY)
    {
        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
        minY = maxY = vertices[0].y;
        for (int i = 1; i < vertices.Length; i++)
        {
            float y = vertices[i].y;
            if (y < minY)
                minY = y;
            if (y > maxY)
                maxY = y;
        }
    }
}
