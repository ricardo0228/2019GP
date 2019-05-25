using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spacecraftMove : MonoBehaviour
{
    private List<Vector3> points = new List<Vector3>();
    private readonly float acc = 0.001f;
    private int num = 1000;
    public LineRenderer line;
    public float GM = 10.0f;
    public float SpeedUp = 1.0f;
    public Transform center;
    public Transform velocity;
    private Vector3 r;
    private Vector3 v;
    private enum TrajectoryType { Oval, Parabola, Hyperbola };
    private TrajectoryType type;
    private float w;
    private float h;
    private float f;
    private float alpha;
    private Vector3 Xi;
    private Vector3 Yj;
    private float T;
    private List<Vector2> TimeMap = new List<Vector2>();
    private float t;
    private float t0;

    private void Awake()
    {
        t = 0;
    }

    void FixedUpdate()
    {
        if (r != this.transform.position - center.position ||
           v != velocity.position - this.transform.position)
        {
            Analyse();
            //AddPoints();
            t = 0;
            InitTime();
        }
        AddPoints();

        t += Time.deltaTime * SpeedUp;

        if (Vector3.Cross(r, v).z > 0)
            SetPosition(t0 + t);
        else
            SetPosition(t0 - t);
    }

    private void AddPoints()
    {
        points.Clear();
        for (int i = 0; i <= num; i++)
        {
            float angle, x = 0, y = 0;
            Vector3 pt = Vector3.zero;

            if (type == TrajectoryType.Oval)
            {
                angle = i * 2.0f * Mathf.PI / num;
                x = w * Mathf.Cos(angle);
                y = h * Mathf.Sin(angle);
                pt = new Vector3(x - f, y, 0);
            }
            else if (type == TrajectoryType.Hyperbola)
            {
                angle = i * 2.0f * Mathf.Acos(w / f) / (num + 1) - Mathf.Acos(w / f);
                float p = 1 / Mathf.Sqrt((Mathf.Pow(Mathf.Cos(angle), 2) / (w * w) - Mathf.Pow(Mathf.Sin(angle), 2) / (h * h)));
                x = -p * Mathf.Cos(angle);
                y = p * Mathf.Sin(angle);
                pt = new Vector3(x + f, y, 0);
            }
            else
            {
                angle = i * 2.0f * Mathf.PI / (num + 1) - Mathf.PI;
                float p = 4 * f * Mathf.Cos(angle) / Mathf.Pow(Mathf.Sin(angle), 2);
                x = -p * Mathf.Cos(angle);
                y = p * Mathf.Sin(angle);
                pt = new Vector3(x + f, y, 0);
            }

            //此时的点其实还都在平面坐标系上
            Quaternion q = Quaternion.AngleAxis(alpha, new Vector3(0, 0, 1));
            pt = q * pt;

            //转换到三维坐标系
            pt = pt.x * Xi + pt.y * Yj;
            pt = pt + center.transform.position;

            points.Add(pt);
        }
        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());
    }

    private void Analyse()
    {
        r = this.transform.position - center.position;
        v = velocity.position - this.transform.position;
        float u = 2 / r.magnitude - v.sqrMagnitude / GM;

        if (u > acc)
        {
            //此时为椭圆
            type = TrajectoryType.Oval;
            SimulateOval();
        }
        else if (u < -acc)
        {
            //此时为双曲线
            type = TrajectoryType.Hyperbola;
            SimulateHyperbola();
        }
        else
        {
            //此时为抛物线
            type = TrajectoryType.Parabola;
            SimulateParabola();
        }

        Vector3 n = Vector3.Cross(v, r);
        Yj = Vector3.Cross(r, n);
        Yj = Yj / Yj.magnitude;
        //这里需要反一下，别问我为什么，实用主义万岁
        if (n.z > 0) Yj = -Yj;
        Xi = r / r.magnitude;

        //print(new Vector3(w, h, f));
        //print(alpha);
    }

    private void SimulateOval()
    {
        //利用轨道线速度公式求出轨道半长轴
        w = 1 / (2 / r.magnitude - v.sqrMagnitude / GM);
        //利用开普勒第二定律求出轨道半短轴
        h = Vector3.Cross(v, r).magnitude * Mathf.Sqrt(w / GM);
        //求出焦距
        f = Mathf.Sqrt(w * w - h * h);

        //利用准线求出x0的值
        float x0 = w * w / f - w / f * r.magnitude;
        //理论上r^2大于等于(x0-f)^2，但是由于精度问题可能略小于0，因此使用Abs函数
        float y0 = Mathf.Sqrt(Mathf.Abs(r.magnitude * r.magnitude - (x0 - f) * (x0 - f)));
        Vector3 r0 = new Vector3(x0 - f, y0, 0);
        //Unity用的是左手坐标系，用夹角来确定y0应该为正还是为负
        if (Vector3.Cross(v, r).z < 0)
        {
            //v0是在当前r0情况下应该具有的速度
            Vector3 v0 = new Vector3(-y0 * w / h, x0 * h / w, 0);
            if (Mathf.Abs(Vector3.Angle(v, r) - Vector3.Angle(v0, r0)) > acc)
                y0 = -y0;
        }
        else
        {
            Vector3 v0 = new Vector3(y0 * w / h, -x0 * h / w, 0);
            if (Mathf.Abs(Vector3.Angle(v, r) - Vector3.Angle(v0, r0)) > acc)
                y0 = -y0;
        }

        Vector3 rr = new Vector3(r.magnitude, 0, 0);
        //先求出在正坐标系中与质心距离为r的点的矢量r0，然后测量需要倾斜alpha使行星可以位于轨道上
        r0 = new Vector3(x0 - f, y0, 0);
        alpha = Vector3.Angle(r0, rr);
        if (Vector3.Cross(r0, rr).z < 0) alpha = -alpha;
    }

    private void SimulateHyperbola()
    {
        //利用轨道线速度公式求出轨道半长轴
        w = -1 / (2 / r.magnitude - v.sqrMagnitude / GM);
        //利用开普勒第二定律求出轨道半短轴
        h = Vector3.Cross(v, r).magnitude * Mathf.Sqrt(w / GM);
        //求出焦距
        f = Mathf.Sqrt(w * w + h * h);

        //利用准线求出x0的值
        float x0 = -w * w / f - w / f * r.magnitude;
        //理论上r^2大于等于(x0-f)^2，但是由于精度问题可能略小于0，因此使用Abs函数
        float y0 = Mathf.Sqrt(Mathf.Abs(r.magnitude * r.magnitude - (x0 + f) * (x0 + f)));
        Vector3 r0 = new Vector3(x0 + f, y0, 0);
        //Unity用的是左手坐标系，用夹角来确定y0应该为正还是为负
        if (Vector3.Cross(v, r).z < 0)
        {
            //v0是在当前r0情况下应该具有的速度
            Vector3 v0 = new Vector3(-y0 * w / h, -x0 * h / w, 0);
            if (Mathf.Abs(Vector3.Angle(v, r) - Vector3.Angle(v0, r0)) > acc)
                y0 = -y0;
        }
        else
        {
            Vector3 v0 = new Vector3(y0 * w / h, x0 * h / w, 0);
            if (Mathf.Abs(Vector3.Angle(v, r) - Vector3.Angle(v0, r0)) > acc)
                y0 = -y0;
        }

        Vector3 rr = new Vector3(r.magnitude, 0, 0);
        //先求出在正坐标系中与质心距离为r的点的矢量r0，然后测量需要倾斜alpha使行星可以位于轨道上
        r0 = new Vector3(x0 + f, y0, 0);
        alpha = Vector3.Angle(r0, rr);
        if (Vector3.Cross(r0, rr).z < 0) alpha = -alpha;
    }

    private void SimulateParabola()
    {
        //利用轨道线速度公式求出轨道半长轴
        w = 0;
        //利用开普勒第二定律求出轨道半短轴
        h = Vector3.Cross(v, r).sqrMagnitude / 2 / GM;
        //求出焦距
        f = h;

        //利用准线求出x0的值
        float x0 = -r.magnitude + f;
        //理论上r^2大于等于(x0-f)^2，但是由于精度问题可能略小于0，因此使用Abs函数
        float y0 = Mathf.Sqrt(Mathf.Abs(r.magnitude * r.magnitude - (x0 + f) * (x0 + f)));
        Vector3 r0 = new Vector3(x0 + f, y0, 0);
        //Unity用的是左手坐标系，用夹角来确定y0应该为正还是为负
        if (Vector3.Cross(v, r).z < 0)
        {
            //v0是在当前r0情况下应该具有的速度
            Vector3 v0 = new Vector3(-y0, 2 * f, 0);
            if (Mathf.Abs(Vector3.Angle(v, r) - Vector3.Angle(v0, r0)) > acc)
                y0 = -y0;
        }
        else
        {
            Vector3 v0 = new Vector3(y0, -2 * f, 0);
            if (Mathf.Abs(Vector3.Angle(v, r) - Vector3.Angle(v0, r0)) > acc)
                y0 = -y0;
        }

        Vector3 rr = new Vector3(r.magnitude, 0, 0);
        //先求出在正坐标系中与质心距离为r的点的矢量r0，然后测量需要倾斜alpha使行星可以位于轨道上
        r0 = new Vector3(x0 + f, y0, 0);
        alpha = Vector3.Angle(r0, rr);
        if (Vector3.Cross(r0, rr).z < 0) alpha = -alpha;
    }

    private void InitTime()
    {
        //先计算出周期T
        T = 2 * Mathf.PI * w * Mathf.Sqrt(w / GM);

        //初始t0由r和alpha决定
        float epsilon = Mathf.Acos((w - r.magnitude) / f);
        if ((w - r.magnitude) / f > 1)
            epsilon = 0;
        if (alpha < 0)
            epsilon = -epsilon;

        t0 = Rad2Time(-epsilon);
    }

    private void SetPosition(float time)
    {
        if (type != TrajectoryType.Oval) return;

        //将t调整到周期内
        while (time > T / 2) time -= T;
        while (time < -T / 2) time += T;

        //求出时间t对应的beta
        float beta = 0;
        if (time > 0) beta = Dichotomy(0, Mathf.PI, time);
        else beta = Dichotomy(-Mathf.PI, 0, time);

        //由beta求出在XOY坐标系的r和v
        float absR = w * (1 - f / w * Mathf.Cos(beta));
        float x0 = w * w / f - w / f * absR;
        float y0 = Mathf.Sqrt(Mathf.Abs(absR * absR - (x0 - f) * (x0 - f)));
        if (beta < 0)
            y0 = -y0;
        Vector3 rt = new Vector3(x0 - f, y0, 0);
        Vector3 vt = new Vector3(-y0 * w / h, x0 * h / w, 0);
        vt = vt / vt.magnitude * Mathf.Sqrt(GM * (2 / rt.magnitude - 1 / w));
        if (Vector3.Cross(rt, vt).z * Vector3.Cross(r, v).z < 0)
            vt = -vt;

        //在XOY中绕焦点旋转
        Quaternion q = Quaternion.AngleAxis(alpha, new Vector3(0, 0, 1));
        rt = q * rt;
        vt = q * vt;

        //转换到三维坐标系
        r = rt.x * Xi + rt.y * Yj;
        v = vt.x * Xi + vt.y * Yj;

        this.transform.position = r + center.transform.position;
        velocity.position = v + this.transform.position;
    }

    //用二分法求弧度，输入时间，输出弧度
    private float Dichotomy(float l, float r, float time)
    {
        if (r - l < acc) return l;

        float ltime = Rad2Time(l);
        float rtime = Rad2Time(r);
        float mtime = Rad2Time((l + r) / 2);

        if ((ltime - mtime) * (time - mtime) >= 0)
            return Dichotomy(l, (l + r) / 2, time);
        return Dichotomy((l + r) / 2, r, time);
    }

    //以弧度为参数的时间方程
    private float Rad2Time(float rad)
    {
        return w * Mathf.Sqrt(w / GM) * (rad - f / w * Mathf.Sin(rad));
    }
}
