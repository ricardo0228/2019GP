using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
// [TODO: SECTION 2.2] Uncomment it.
using VRTK;      /* ------ [Line 4] ------ */

[RequireComponent(typeof(CarController))]
public class VRCarUserControl : MonoBehaviour {

    //// Get the transforms of controllers,     /* ------ [Line 9] ------ */
    [Tooltip("The transform of the left controller")]
    public Transform leftController;
    [Tooltip("The transform of the right controller")]                                              
    public Transform rightController;
    //// and their events       /* ------ [Line 14] ------ */
    // [TODO: SECTION 2.2] Uncomment two controller events below.
    private VRTK_ControllerEvents leftControllerEvents;
    private VRTK_ControllerEvents rightControllerEvents;

    //// The position of the origin steering wheel      /* ------ [Line 19] ------ */
    //// We will use it to define where the plane of steering wheel is
    [Tooltip("The original transform of the steering wheel of the car")]
    public Transform steeringWheel;

    //// Is it in VR controller mode/VR touchpad mod
    [Tooltip(
        @"
          If it is true, the car should be controlled by the touchpad of controllers.
          If it is false, it should be controlled by the transform and trigger of controllers.
        "
    )]
    public bool inTouchPadMode = false;         /* ------ [Line 31] ------ */

    //// We will use it to control our car
    //// Do not need to care about the implementation
    private CarController m_Car;

    //// Data we will calculate for car moving degree in VR controller mode
    private Vector3 xVector, yVector, zVector;      /* ------ [Line 38] ------ */
    private Vector3 leftFootPoint, rightFootPoint;
    private Vector3 steeringWheelVector;
    private float leftFactor, rightFactor;
    //// Data we will calculate for car moving acceleration in VR controller mode
    private float leftPresure = 0, rightPresure = 0;    /* ------ [Line 43] ------ */

    //// Data we will calculate for car moving in VR touchpad mode
    private float leftAxisX = 0, rightAxisY = 0;        /* ------ [Line 46] ------ */

    [Header("For Debug")]
    [Tooltip("[Readonly] The degree of the car, from -90 to 90, where negative means left.")]
    public float degree;        /* ------ [Line 50] ------ */
    [Tooltip("[Readonly] The degree of the car, from -1 to 1, where negative means left.")]
    public float h;             /* ------ [Line 52] ------ */
    [Tooltip("[Readonly] The acceleration of the car, from -1 to 1, where negative means back.")]
    public float v;

    //// Calculate the factor Alpha         /* ------ [Line 56] ------ */
    /// <summary> Calculate the factor Alpha, refer to Equation [4][5] for details. </summary>
    /// <param name="_originalPoint"> original point of steering wheel </param>
    /// <param name="_point"> point of the controller </param>
    /// <param name="_zVector"> normal of the plane of steering wheel </param>
    /// <returns> The factor Alpha </returns>
    private float Factor(Vector3 _originalPoint, Vector3 _point, Vector3 _zVector) {

        //// [TODO: SECTION 2.1] Finish the function
        // Hint: This function is used to calculate the factor,
        //       refer to Equation[4][5] for details
        return (_zVector.x * (_originalPoint.x - _point.x) + _zVector.y * (_originalPoint.y - _point.y) + _zVector.z * (_originalPoint.z - _point.z)) /
                    (_zVector.x * _zVector.x + _zVector.y * _zVector.y + _zVector.z + _zVector.z);




    }

    //// When trigger axis of leftController was changed        /* ------ [Line 75] ------ */
    // [TODO: SECTION 2.2] Uncomment the function
    private void OnLeftTriggerAxisChanged(object sender, ControllerInteractionEventArgs e) {
        leftPresure = e.buttonPressure;
    }

    //// When trigger axis of rightController was changed
    // [TODO: SECTION 2.2] Uncomment the function
    private void OnRightTriggerAxisChanged(object sender, ControllerInteractionEventArgs e) {
        rightPresure = e.buttonPressure;
    }

    //// [TODO: SECTION 4.3] Define all functions for delegate events if you need
    // Hint: You may need:
    //       1. The function called when left touchpad axis was changed.
    //       2. The function called when right touchpad axis was changed.
    //       3. The function called when button two was pressed.
    private void OnLeftTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
    {
        leftAxisX = e.touchpadAxis.x;
    }

    private void OnRightTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
    {
        rightAxisY = e.touchpadAxis.y;
    }

    private void OnButtonTwoPressed(object sender, ControllerInteractionEventArgs e)
    {
        inTouchPadMode = !inTouchPadMode;
    }

    private void Awake() {
        ////// Get the car controller
        m_Car = GetComponent<CarController>();

        ////// Get controller events
        if (leftController != null) {
            ////// Init left controller and regist functions on delegate events
            //// [TODO: SECTION 2.2] Uncomment codes below.         /* ------ [Line 114] ------ */
            leftControllerEvents = leftController.GetComponent<VRTK_ControllerEvents>();
            leftControllerEvents.TriggerAxisChanged += OnLeftTriggerAxisChanged;

            //// [TODO: SECTION 4.3] Regist functions on delegate events if you need.
            // Hint: You may need:
            //       1. Touchpad Axis Changed Event
            //       2. Button Two Pressed Event
            leftControllerEvents.TouchpadAxisChanged += OnLeftTouchpadAxisChanged;
            leftControllerEvents.ButtonTwoPressed += OnButtonTwoPressed;


        }
        if (rightController != null) {
            ////// Init right controller and regist function on delegate event
            //// [TODO: SECTION 2.2] Uncomment codes below.         /* ------ [Line 128] ------ */
            rightControllerEvents = rightController.GetComponent<VRTK_ControllerEvents>();
            rightControllerEvents.TriggerAxisChanged += OnRightTriggerAxisChanged;

            //// [TODO: SECTION 4.3] Regist functions on delegate events if you need.
            // Hint: You may need:
            //       1. Touchpad Axis Changed Event
            //       2. Button Two Pressed Event
            rightControllerEvents.TouchpadAxisChanged += OnRightTouchpadAxisChanged;
            rightControllerEvents.ButtonTwoPressed += OnButtonTwoPressed;


        }
    }

    private void FixedUpdate() {
        ////// Make sure these institutes are not null
        if (leftController == null || rightController == null || steeringWheel == null) {
            return;
        }

        if (inTouchPadMode) {
            ////// Use touchpad to control the car
            //// [TODO: SECTION 4.3] Update h, v and degree.
            v = rightAxisY;
            h = leftAxisX;
            degree = h * 90f;
        }
        else {
            ////// Use trigger to calculate the acceleration of car     /* ------ [Line 157] ------ */
            v = rightPresure - leftPresure;

            ////// Use transforms of left/rightControllers to calculate the degree of car
            //// Get x-Axis, y-Axis and z-Axis(AKA normal) of steering wheel plane      /* ------ [Line 161] ------ */
            xVector = steeringWheel.right;
            yVector = steeringWheel.up;
            zVector = steeringWheel.forward;

            //// [TODO: SECTION 2.1]: Finish the calculation    /* ------ [Line 166] ------ */
            // Hint: 1. Use function "Factor" to calculate leftFactor & rightFactor
            //       2. Use Equation [6] to calculate leftFootPoint & rightFootPoint
            leftFactor = Factor(steeringWheel.position, leftController.position, zVector);
            rightFactor = Factor(steeringWheel.position, rightController.position, zVector);

            leftFootPoint = leftController.position + leftFactor * zVector;
            rightFootPoint = rightController.position + rightFactor * zVector;

            //// Calculate steering wheel vector    /* ------ [Line 175] ------ */
            steeringWheelVector = rightFootPoint - leftFootPoint;

            //// Calculate degree(-90~90)
            degree = Mathf.Min(Vector3.Angle(steeringWheelVector, yVector), 90f);
            if (Vector3.Angle(steeringWheelVector, xVector) < 90f) {
                degree = -degree;
            }
            //// and h(-1~1)
            h = degree / 90f;
        }

        ////// move the car
        m_Car.Move(h, v, v, 0f);
    }
}