using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    
    public KeyCode keyRight;
    public KeyCode keyLeft;

    //// current velocity
    private AngleController AC;
    private float horizontalVel = 0.0f;
    private float forwardVel = 10.0f;
    private float totalTime = 0.0f;

    void Start()
    {
        AC = GetComponent<AngleController>();
    }

    private void Awake()
    {
        GameManager.Instance.UpdateSpeed(forwardVel);
    }

    void Update()
    {
        //// control with keyboard
        if (Input.GetKey(keyLeft))
            horizontalVel = -3.0f;
        else if (Input.GetKey(keyRight))
            horizontalVel = +3.0f;
        else
            horizontalVel = 0.0f;
        //// TODO: Your Implementation:
        //// - Update the horizontal velocity with angleController
        //// When not dead, update velocity
        float k = (int)(totalTime / 5) / 10.0f + 1;

        if (!GameManager.Instance.IsRunning()) {
            k = 0;
        }
        else
        {
            AC.Update();
            horizontalVel = AC.movingSpeed;
            totalTime += Time.deltaTime;
        }

        this.transform.GetComponent<Rigidbody>().velocity = new Vector3(horizontalVel * k, 0.0f, forwardVel * k);
        GameManager.Instance.UpdateSpeed(forwardVel * k);

    }
    
    void OnTriggerEnter(Collider other) {
        //// TODO: Your Implementation:
        //// - When collide with obj with tag 'CollisionWall' or 'FallWall', trigger OnDeath() in GameManager

        if (other.CompareTag("CollisionWall") || other.CompareTag("FallWall"))
        {
            GameManager.Instance.OnDeath(true);
        }
    }
}
