using UnityEngine;
using System.Collections.Generic;
using Devices;
using System;

public class InputManager : MonoBehaviour {
    BmcmSensor sensor;

    float rawVal_left       = 0;
    float rawVal_right      = 0;
    float rawVal_ripcord    = 0;

    float normalizedVal_left = 0;
    float normalizedVal_right = 0;
    float normalizedVal_ripcord = 0;

    float oldNormalizedVal_left = 0;
    float oldNormalizedVal_right = 0;

    float minRawValue_leftgrip = 0;
    float maxRawValue_leftgrip = 5.14f;

    float minRawValue_rightgrip = 0;
    float maxRawValue_rightgrip = 5.14f;

    float minRawVal_ripcord = 0;
    float maxRawVal_ripcord = 5.14f;

    private float threshold_move = 0;
    private float threshold_pull = 0;

    private bool showInputGUI = false;
    private bool enableKeyboard = false;

    int port_ripcord = 1;
    int port_leftGrip = 1;
    int port_rightGrip = 1;
    int line_ripcord = 1;
    int line_leftgrip = 1;
    int line_rightgrip = 1;

    private void Awake()
    {
        Configuration.LoadConfig();

        sensor = new BmcmSensor("usb-ad");
        sensor.Init();

    }

    // Use this for initialization
    void Start()
    {
        port_rightGrip  = Convert.ToInt32(Configuration.GetAttricuteByTagName("ripcord", "port"));
        port_leftGrip   = Convert.ToInt32(Configuration.GetAttricuteByTagName("leftgrip", "port"));
        port_rightGrip = Convert.ToInt32(Configuration.GetAttricuteByTagName("rightgrip", "port"));

        line_ripcord = Convert.ToInt32(Configuration.GetAttricuteByTagName("ripcord", "line"));
        line_leftgrip = Convert.ToInt32(Configuration.GetAttricuteByTagName("leftgrip", "line"));
        line_rightgrip = Convert.ToInt32(Configuration.GetAttricuteByTagName("rightgrip", "line"));

        minRawValue_leftgrip = (float)Configuration.GetInnerTextByTagName("minRawValue_leftgrip", minRawValue_leftgrip);
        minRawValue_rightgrip = (float)Configuration.GetInnerTextByTagName("minRawValue_rightgrip", minRawValue_rightgrip);

        maxRawValue_leftgrip = (float)Configuration.GetInnerTextByTagName("maxRawValue_leftgrip", maxRawValue_leftgrip);
        maxRawValue_rightgrip = (float)Configuration.GetInnerTextByTagName("maxRawValue_rightgrip", maxRawValue_rightgrip);

        minRawVal_ripcord = (float)Configuration.GetInnerTextByTagName("minRawVal_ripcord", minRawVal_ripcord);
        maxRawVal_ripcord = (float)Configuration.GetInnerTextByTagName("maxRawVal_ripcord", maxRawVal_ripcord);

        threshold_move = (float)Configuration.GetInnerTextByTagName("threshold_move", threshold_move);
        threshold_pull = (float)Configuration.GetInnerTextByTagName("threshold_pull", threshold_pull);

        enableKeyboard = Configuration.GetInnerTextByTagName("debug", false);

        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            enableKeyboard = !enableKeyboard;
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            showInputGUI = !showInputGUI;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (sensor.IsValid())
        {
            float rawVal_ripcord    = sensor.GetAnalogIn(port_rightGrip, line_ripcord);
            float rawVal_left       = sensor.GetAnalogIn(port_leftGrip, line_leftgrip);
            float rawVal_right      = sensor.GetAnalogIn(port_rightGrip, line_rightgrip);

            if (Input.GetKey(KeyCode.C))
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    minRawValue_leftgrip = rawVal_left;
                    minRawValue_rightgrip = rawVal_right;
                    minRawVal_ripcord = rawVal_ripcord;

                    Configuration.SaveValueInConfig(minRawValue_leftgrip.ToString(), "minRawValue_leftgrip");
                    Configuration.SaveValueInConfig(minRawValue_rightgrip.ToString(), "minRawValue_rightgrip");
                    Configuration.SaveValueInConfig(minRawVal_ripcord.ToString(), "minRawVal_ripcord");
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    maxRawValue_leftgrip = rawVal_left;
                    maxRawValue_rightgrip = rawVal_right;

                    Configuration.SaveValueInConfig(maxRawValue_leftgrip.ToString(), "maxRawValue_leftgrip");
                    Configuration.SaveValueInConfig(maxRawValue_rightgrip.ToString(), "maxRawValue_rightgrip");
                }
                else if (Input.GetKeyDown(KeyCode.P))
                {
                    maxRawVal_ripcord = rawVal_ripcord;
                    Configuration.SaveValueInConfig(maxRawVal_ripcord.ToString(), "maxRawVal_ripcord");
                }
            }

            //normalized Values
            normalizedVal_left = Remap(rawVal_left, minRawValue_leftgrip, maxRawValue_leftgrip, -1, 1);
            normalizedVal_right = Remap(rawVal_right, minRawValue_rightgrip, maxRawValue_rightgrip, -1, 1);
            normalizedVal_ripcord = Remap(rawVal_ripcord, minRawVal_ripcord, maxRawVal_ripcord, 0, 1);

            //noise value filter
            normalizedVal_left = ReduceResolution(normalizedVal_left, oldNormalizedVal_left);
            normalizedVal_right = ReduceResolution(normalizedVal_right, oldNormalizedVal_right);

            oldNormalizedVal_left = normalizedVal_left;
            oldNormalizedVal_right = normalizedVal_right;
        }         
    }

    float ReduceResolution(float newVal, float oldVal){
        if (Mathf.Abs(normalizedVal_left - oldNormalizedVal_left) < threshold_move)
        {
            return oldVal;
        }
        return newVal;
     }

    float Remap(float _val, float _minIn, float _maxIn, float _minOut, float _maxOut)
    {
        return _minOut + (_maxOut - _minOut) * (_val - _minIn) / (_maxIn - _minIn);
    }

    void OnGUI()
    {
        if (showInputGUI)
        {
            int yPos = 0, lineHeight = 20;
            int x = 20; //Screen.width / 2;
            int y = 380;

            GUI.Box(new Rect(10, y, 200, 200), "");
            GUI.Label(new Rect(x, y + (++yPos) * lineHeight, 500, 500), "USB-AD is "            + sensor.IsValid() );
            GUI.Label(new Rect(x, y + (++yPos) * lineHeight, 500, 500), "raw Value left: "      + rawVal_left);
            GUI.Label(new Rect(x, y + (++yPos) * lineHeight, 500, 500), "norm Value left: "     + normalizedVal_left);
            GUI.Label(new Rect(x, y + (++yPos) * lineHeight, 500, 500), "raw Value right: "     + rawVal_right);
            GUI.Label(new Rect(x, y + (++yPos) * lineHeight, 500, 500), "norm Value right: "    + normalizedVal_right);
            GUI.Label(new Rect(x, y + (++yPos) * lineHeight, 500, 500), "norm Value ripcord: "  + normalizedVal_ripcord);
        }
    }

    // move to left or right
    public float GetHorinatalAxes()
    {
        if (enableKeyboard)
        {
            return Input.GetAxis("Horizontal");
        }

        return (normalizedVal_left - normalizedVal_right) * 0.5f;
    }

    // move to up or down
    public float GetVerticalAxes()
    {
        if (enableKeyboard)
        {
            return Input.GetAxis("Vertical");
        }

        return (normalizedVal_left + normalizedVal_right) * 0.5f;
    }

    public bool PullRipcord()
    {
        if (enableKeyboard)
        {
            return Input.GetKeyDown(KeyCode.Space);
        }

        if (normalizedVal_ripcord > threshold_pull)
        {
            return true;
        }

        return false;
    }

}
