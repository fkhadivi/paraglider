using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {


    private float x_inputval = 0;
    private float y_inputval = 0;

    public float xValue = 0;
    public float yValue = 0;

    private bool joystickIsUsed = false;
    private bool showInputGUI = false;
    private bool usingCalibration = false;
    // Use this for initialization
    void Start()
    {

        if (Input.GetJoystickNames().Length > 0)
            joystickIsUsed = true;

        Cursor.visible = false;
    }
    string lastTag = "";
    
    // Update is called once per frame
    void Update()
    {
        xValue = 0;
        yValue = 0;

        if (Input.GetKeyDown(KeyCode.F5))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            showInputGUI = true;
            usingCalibration = true;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            showInputGUI = false;
            usingCalibration = false;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            showInputGUI = !showInputGUI;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (!joystickIsUsed)
        {
            if (Time.timeScale == 1)
            {
                xValue = Input.GetAxis("Horizontal");
                yValue = Input.GetAxis("Vertical");
            }
            return;
        }

        x_inputval = Input.GetAxis("X-Axes");
        y_inputval = Input.GetAxis("Y-Axes");
        //if (joystickIsUsed && !Input.anyKey)

        if (!usingCalibration && Time.timeScale == 1)
        {
                xValue = x_inputval;
                yValue = y_inputval;
            
        }
    }

    void OnGUI()
    {
        if (showInputGUI)
        {
            int yPos = 0, lineHeight = 20;
            int x = 20; //Screen.width / 2;
            int y = 380;

            GUI.Box(new Rect(10, y, 200, 200), "");
            GUI.Label(new Rect(x, y + (++yPos) * lineHeight, 500, 500), "Input Device: Joystick");
            GUI.Label(new Rect(x, y + (++yPos) * lineHeight, 500, 500), "X raw: " + Input.GetAxis("X-Axes"));
            GUI.Label(new Rect(x, y + (++yPos) * lineHeight, 500, 500), "X    : " + x_inputval);
            GUI.Label(new Rect(x, y + (++yPos) * lineHeight, 500, 500), "Y raw: " + (Input.GetAxis("Y-Axes")));
            GUI.Label(new Rect(x, y + (++yPos) * lineHeight, 500, 500), "Y    : " + y_inputval);
        }
    }

    bool canPlayGame = false;
    List<string> tagCodesUsed = new List<string>();
    void CheckTagCode(string _code) {
    var codeAllreadyInUse = false;

    foreach (string tagcode in tagCodesUsed) {
        if (tagcode == _code)  {
            codeAllreadyInUse = true;
        }
    }

    if (codeAllreadyInUse) {
        Debug.Log("this tag was allready in use, do nothing");
    } else {
        Debug.Log("new tag, save it in array and redirect to instruction");

        if (tagCodesUsed.Count >= 2)
            tagCodesUsed.RemoveAt(0);
        
        tagCodesUsed.Add(_code);

        canPlayGame = true;
    }
    }

    void OnDestroy()
    {
      
    }

}
