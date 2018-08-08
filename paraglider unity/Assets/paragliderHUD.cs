using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class paragliderHUD : MonoBehaviour {

    public string levelString = "Welt 0/4";
    public Text levelInfoText;

    public Image timePieFill;
    public float timePieFillAmount;

    public ParagliderControler glider;

    public RectTransform speedoHand;
    public float speedoMinAngle;
    public float speedoMaxAngle;
    public float speedoMin;
    public float speedoMax;
    public float speedoValue;
    public Text speedDigits;
    public string speedUnit = "km/h";

    public RectTransform altChangeHand;
    public float altChangeMinAngle;
    public float altChangeMaxAngle;
    public float altChangeMin;
    public float altChangeMax;
    public float altChangeValue;
    public float altitudeValue;
    public Text altDigits;
    public string altUnit = "m";



    // Use this for initialization
    void Start () {
		
	}
	


    public void appear(bool shouldAppear)
    {
        
    }

    public void updatespeedo()
    {
        if (glider != null)
        {
            speedoValue = glider.speed;
            altitudeValue = glider.altitude;
            altChangeValue = glider.altChange;

            speedoHand.localEulerAngles = new Vector3(0, 0, BenjasMath.map(speedoValue, speedoMin, speedoMax, speedoMinAngle, speedoMaxAngle));
            speedDigits.text = Mathf.FloorToInt(speedoValue).ToString() + " " + speedUnit;

            altChangeHand.localEulerAngles = new Vector3(0, 0, BenjasMath.map(altChangeValue, altChangeMin, altChangeMax, altChangeMinAngle, altChangeMaxAngle));
            altDigits.text = Mathf.FloorToInt(altitudeValue).ToString() + " " + altUnit;
        }
    }

    public void setGameTime(float time0to1)
    {
        timePieFill.fillAmount = Mathf.Clamp01(time0to1);

    }

    public void setLevelString(string levelString)
    {
        levelInfoText.text = levelString;
    }


    // Update is called once per frame
    void Update () {
        updatespeedo();
        //setGameTime(timePieFillAmount);
    }
}
