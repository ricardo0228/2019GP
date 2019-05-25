using UnityEngine;

public class SteeringWheelControl : MonoBehaviour {
    //// VRCarUserControl on the Car
    [Tooltip("VR user control scripts on the car")]
    public VRCarUserControl vrCarUserCtrl;

    //// record the original local rotation
    private Quaternion origin;

    //// Use this for initialization
    void Start() {
        //// [TODO: SECTION 4.2]
        // Hint: Record the original local rotation
        origin = this.transform.localRotation;
    }

    // Update is called once per frame
    void Update() {
        //// [TODO: SECTION 4.2]
        // Hint: Update current local rotation by degree from vrCarUserCtrl
        transform.localRotation = origin * Quaternion.Euler(0, 0, vrCarUserCtrl.degree);
    }
}
