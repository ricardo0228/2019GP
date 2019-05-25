using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairController : MonoBehaviour
{
    public GameObject prefab;               //头发预制体
    public Transform head;                  //头部
    public float nodeLength = 0.07f;         //节点定长
    public float damping = 0.95f;           //阻尼系数
    public int num = 30;                    //头发密度
    private Vector3 a = new Vector3(0, -9.8f, 0);   //加速度
    private float headExpend = 0.1f;       //头部扩张
    private List<GameObject> hairs = new List<GameObject>();
    void Start()
    {
        float R = head.localScale.x / 2 + headExpend;
        Vector3 center = head.position;
        
        for (int i = 0; i <= num; i++)
        {
            for(int j = 0; j <= num / 2; j++)
            {
                Vector3 root = new Vector3(0, 0, 0);
                root.x = 2 * R / num * i - R;
                root.z = 2 * R / num * j;
                float length = Vector3.Magnitude(center - root);
                if(length <= R)
                {
                    root.y = Mathf.Sqrt(R * R - length * length);
                    GameObject go = Instantiate(prefab) as GameObject;
                    go.GetComponent<HairRender>().head = head;
                    go.GetComponent<HairRender>().hairRoot = root;
                    go.GetComponent<HairRender>().nodeLength = nodeLength;
                    go.GetComponent<HairRender>().damping = damping;
                    hairs.Add(go);
                }
            }
        }
    }
    
    void Update()
    {
        if (Input.GetKey("z"))
            a.z += 0.3f;
        if (Input.GetKey("c"))
            a.z -= 0.3f;

        foreach (GameObject e in hairs)
        {
            e.GetComponent<HairRender>().a = a;
        }   
    }
}
