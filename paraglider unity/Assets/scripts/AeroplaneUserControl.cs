using UnityEngine;

[RequireComponent(typeof(AeroplaneController))]
public class AeroplaneUserControl : MonoBehaviour
{
	// reference to the aeroplane that we're controlling
	private AeroplaneController aeroplane;
    private InputManager inputmanager;
    public bool isPlaying = true;

    float rollValueFactor = 1.0f;
    float pitchValueFactor = 1.0f;

    void Awake ()
    {
        // Set up the reference to the aeroplane controller.
        aeroplane = GetComponent<AeroplaneController>();
        inputmanager = transform.parent.GetComponentInChildren<InputManager>();
    }

    void Start()
    {
        rollValueFactor     = (float)Configuration.GetInnerTextByTagName("rollValueFactor", rollValueFactor);
        pitchValueFactor    = (float)Configuration.GetInnerTextByTagName("pitchValueFactor", pitchValueFactor);
    }

    void FixedUpdate()
    {
        // Read input for the pitch, yaw, roll and throttle of the aeroplane.
        float roll = 0;
		float pitch = 0;
        float yaw   = 0;
        float throttle = 1; 

        if (inputmanager)
        {
            if (isPlaying)
            {
                roll = inputmanager.GetResultLeftRightMinus1To1();
                pitch = inputmanager.GetResultUpDownMinus1To1() * -1.0f;

                roll *= rollValueFactor;
                pitch *= pitchValueFactor;
            }
        }

        // Pass the input to the aeroplane
        aeroplane.Move(roll, pitch, yaw, throttle, false);
    }

}
