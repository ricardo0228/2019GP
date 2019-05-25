using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePostion : MonoBehaviour
{
    public Transform lastNodeTran;   //上一个节点的位置
    float nodeDistance = 0.1f;  //节点定长
    float damping = 0.8f;  //阻尼系数
    float realDistance;      //真实距离
    Rigidbody rig;

    struct Node
    {
        public Vector3 p0, p1; // 前帧/本帧的位置
        public float length;   // 和上一节点的止动长度
    };
    private Node node;

    private void Start()
    {
        node.p0 = this.transform.position;
        node.p1 = this.transform.position;
        node.length = nodeDistance;
    }

    private void Update()
    {
        Simulate();
        this.transform.position = node.p1;
    }

    void Simulate()
    {
        Vector3 a = new Vector3(0, -9.8f, 0); // 现时只是引力加速度常量
        Vector3 p2 = Verlet(node.p0, node.p1, damping, a, Time.deltaTime);
        node.p0 = node.p1;
        node.p1 = p2; // 以新状态取代旧状态
        Node tmp = LengthConstraint(lastNodeTran.position, node.p1, node.length);
        lastNodeTran.position = tmp.p0;
        node.p1 = tmp.p1;
    }

    Vector3 Verlet(Vector3 p0, Vector3 p1, float d, Vector3 a, float dt)
    {
        Vector3 p2 = p1 + d * (p1 - p0) + a * dt * dt;
        return p2;
    }

    Node LengthConstraint(Vector3 x1, Vector3 x2, float maxLength) //减速
    {
        float realLen = Vector3.Magnitude(x1 - x2);
        Vector3 x3 = x1 + (x2 - x1) * (realLen - maxLength) / (2 * realLen);
        Vector3 x4 = x2 - (x2 - x1) * (realLen - maxLength) / (2 * realLen);
        Node tmp = new Node();
        tmp.p0 = x3;
        tmp.p1 = x4;
        return tmp;
    }
}
