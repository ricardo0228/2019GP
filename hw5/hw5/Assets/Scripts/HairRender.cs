using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairRender : MonoBehaviour
{
    public Vector3 hairRoot;                //发根
    public Transform head;                  //头部
    public float nodeLength = 0.1f;         //节点定长
    public float damping = 0.9f;            //阻尼系数
    public Vector3 a = new Vector3(0, -9.8f, 0);   //加速度
    private float headExpend = 0.05f;       //头部扩张
    LineRenderer lineRen;

    struct Node
    {
        public Vector3 p0, p1; // 前帧/本帧的位置
    };
    struct Strand
    {
        public List<Node> nodes; // 此发束中，节点数组的起始和结束索引
        public Vector3 rootP;    // 发根的局部坐标（相对于头的变换）
    };
    Strand strand;

    private void Start()
    {
        lineRen = GetComponent<LineRenderer>();
        lineRen.positionCount = 20;

        this.transform.position = new Vector3(0, 0, 0);
        
        strand.nodes = new List<Node>();
        for (int i = 1; i <= 20; i++)
        {
            Node blankNode;
            blankNode.p0 = new Vector3(0, 0, nodeLength * i);
            blankNode.p1 = new Vector3(0, 0, nodeLength * i);
            strand.nodes.Add(blankNode);
        }
        strand.rootP = hairRoot;
    }

    private void Update()
    {
        Simulate();

        Vector3[] postions = new Vector3[20];
        for (int i = 0; i < 20; i++)
        {
            postions[i] = strand.nodes[i].p1;
        }
        lineRen.SetPositions(postions);
    }

    void Simulate()
    {
        for (int i = 0; i < 20; i++)
        {
            //Verlet积分
            Vector3 p2 = Verlet(strand.nodes[i].p0, strand.nodes[i].p1, damping, a, Time.deltaTime);
            //替换
            Node tmpNode;
            tmpNode.p0 = strand.nodes[i].p1;
            tmpNode.p1 = p2;
            strand.nodes[i] = tmpNode;
        }

        for (int i = 0; i < 19; i++)
        {
            Node currentNode = strand.nodes[i];
            Node nextNode = strand.nodes[i + 1];
            //长度约束
            Node result = LengthConstraint(currentNode.p1, nextNode.p1, nodeLength);
            currentNode.p1 = result.p0;
            nextNode.p1 = result.p1;
            //碰撞约束
            currentNode.p1 = CollideConstraint(head, currentNode.p1);

            strand.nodes[i] = currentNode;
            strand.nodes[i + 1] = nextNode;
        }
        //固定发根
        Node rootNode;
        rootNode.p0 = head.TransformPoint(strand.rootP);
        rootNode.p1 = head.TransformPoint(strand.rootP);
        strand.nodes[0] = rootNode;
    }

    Vector3 Verlet(Vector3 p0, Vector3 p1, float d, Vector3 a, float dt) //Verlet积分
    {
        Vector3 p2 = p1 + d * (p1 - p0) + a * dt * dt;
        return p2;
    }

    Vector3 CollideConstraint(Transform head, Vector3 x) //碰撞约束
    {
        Vector3 center = head.position;
        float R = head.localScale.x / 2 + headExpend;

        float distance = Vector3.Magnitude(center - x);
        if(distance < R)
        {
            return center + (x - center) * R / distance;
        }

        return x;
    }

    Node LengthConstraint(Vector3 x1, Vector3 x2, float maxLength) //长度约束
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
