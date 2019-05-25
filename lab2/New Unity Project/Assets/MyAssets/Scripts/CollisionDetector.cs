using UnityEngine;
//// [TODO: SECTION 4.1] Uncomment it.
using VRTK;

public class CollisionDetector : MonoBehaviour {
    //// we get the controllers
    [Tooltip("The left controller game object of VR device")]
    public GameObject leftController;
    [Tooltip("The right controller game object of VR device")]
    public GameObject rightController;
    //// and their reference
    // [TODO: SECTION 4.1] Uncomment two controller references you may need.
    private VRTK_ControllerReference leftControllerReference;
    private VRTK_ControllerReference rightControllerReference;

    //// Handle collision event
    //// [TODO: SECTION 4.1] Trigger haptics when collision happens.
    // Hint 1. Which function should we use? OnCollisionEnter, or OnTriggerEnter?
    //      2. Use static function VRTK_ControllerReference.GetControllerReference(controllers) to get reference
    //      3. Use static function VRTK_ControllerHaptics.TriggerHapticPulse(ctrlRef, str, dura, itvl) to trigger haptics

    public void OnCollisionEnter(Collision other)
    {
        leftControllerReference = VRTK_ControllerReference.GetControllerReference(leftController);
        rightControllerReference = VRTK_ControllerReference.GetControllerReference(rightController);

        VRTK_ControllerHaptics.TriggerHapticPulse(leftControllerReference, 0.1f, 1, 0.05f);
        VRTK_ControllerHaptics.TriggerHapticPulse(rightControllerReference, 0.1f, 1, 0.05f);
    }

}