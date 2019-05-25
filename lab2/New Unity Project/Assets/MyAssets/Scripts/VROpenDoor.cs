using UnityEngine;
//[TODO: SECTION 3.2] Uncomment it.      /* ------ [Line 2] ------ */
using VRTK;

//[TODO: SECTION 3.2] Uncomment the class.
       // ------ [Line 6] ------ //
public class VROpenDoor : VRTK_InteractableObject {

    [Header("[VROpenDoor] Open Settings")]
    //// User Settings          // ------ [Line 10] ------ //
    [Tooltip("The max degree when the door is opened")]
    public float maxDegree = 60;
    [Tooltip("The duration of the door from closed to opened (or the inverse)")]
    public float openDuration = 2.0f;

    //// Record the original local rotation         // ------ [Line 16] ------ //
    private Quaternion origin;

    //// Current status of the door
    private float currTime = 0;
    private bool isOpenning = false;

    protected override void Awake() {       // ------ [Line 23] ------ //
        //// Call Awake() of VRTK_InteractableObject first.
        //// Do not remove it!!!
        base.Awake();

        //// [TODO: SECTION 3.3] Init param(s)
        // Hint: Record the original local rotation
        origin = gameObject.transform.localRotation;

    }

    protected override void Update() {      // ------ [Line 34] ------ //
        //// Call Update() of VRTK_InteractableObject first.
        //// Do not remove it!!!
        base.Update();

        //// [TODO: SECTION 3.3] Update param(s)
        // Hint: 1. Update currTime for different isOpenning
        //       2. Update current local rotation.
        //          Notice: CurrRotation = OriginalRotation * Degree,
        //          and the degree can be calculated by currTime.

        if (isOpenning)
        {
            if (currTime < openDuration)
            {
                currTime += Time.deltaTime;
            }
        }
        else
        {
            if (currTime > 0)
            {
                currTime -= Time.deltaTime;
            }
        }

        float angle = -maxDegree * currTime / openDuration;
        gameObject.transform.localRotation = origin * Quaternion.Euler(angle, 0, 0);




      






    }

    //// Override using event
    public override void StartUsing(VRTK_InteractUse usingObject) {     // ------ [Line 63] ------ //
        //// [TODO: SECTION 3.3] Change param(s) when using event happens.
        // Hint: Change isOpenning when "Using Event" was triggered.
        
            isOpenning = !isOpenning;
       
    }
}

