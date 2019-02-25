using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class paragliderHUD : MonoBehaviour {

    //public string levelInfoString = "Welt 0/4";
    public Text levelInfoText;

    public Image timePieFill;
    public float timePieFillAmount;

    public ParagliderControler glider;

    //public float instrumentSmoothing = 0.5f;
    public float instrumentMaxDegreesPerSec = 1;
    public RectTransform speedoHand;
    public float sppedoSmoothing = 0.9f;
    public float speedoMinAngle;
    public float speedoMaxAngle;
    public float speedoMin;
    public float speedoMax;
    public float speedoValue;
    public Text speedDigits;
    public string speedUnit = "km/h";
    public bool ignoreVerticalSpeed = true;

    public RectTransform altChangeHand;
    public float altChangeSmoothing = 0.9f;
    public Vector2 altChangeSmoothingMinMax = new Vector2(0.8f, 0.99f);
    public Vector2 altChangeSmoothingAltLimits = new Vector2(50, 150);
    public float altChangeMinAngle;
    public float altChangeMaxAngle;
    public float altChangeMin;
    public float altChangeMax;
    public float altChangeValue;
    public float altitudeValue;
    public Text altDigitsRed;
    public Text altDigitsBlack;
    public Text altDigitsWhite;
    public Text altDigitsUnit;
    public string altUnit = "m";

    public experimentaAnimationPresets mapFrame;
    public experimentaAnimationPresets meterFrame;
    public experimentaAnimationPresets meterFrameStraight;
    public experimentaAnimationPresets meterFrameCutout;
    public experimentaAnimationPresets meterInstruments;
    public experimentaAnimationPresets compassBar;
    public experimentaAnimationPresets compassBarMover;
    public experimentaAnimationPresets compassBarShrinker;
    public experimentaAnimationPresets compassForeground;


    // Use this for initialization
    void Start () {
    

}

    public STATE state;
    public STATE currentState;

    public enum STATE
    {
        HIDDEN,
        COMPLETE,
        NO_INFO,
        INFO_ONLY
    }

    /*
    public void toggleVisibility(bool shouldBeVisible)
    {
        state = shouldBeVisible;
        currentState = shouldBeVisible;
        appear(mapFrame,shouldBeVisible);
        appear(meterFrame, shouldBeVisible);
        appear(meterInstruments, shouldBeVisible);
        appear(compassBar, shouldBeVisible);
        appear(compassForeground, shouldBeVisible);
        if(shouldBeVisible) appear(compassBarShrinker, true);
    }

            public void toggleCompassBar(bool shouldBeGrown)
    {
        appear(compassBarShrinker, shouldBeGrown);
    }



   
    */

    public void toggleVisibility()
    {
        switch(state)
        {
            case STATE.HIDDEN:
                appearHIDDEN();
                break;
            case STATE.COMPLETE:
                appearCOMPLETE();
                break;
            case STATE.NO_INFO:
                appearNOINFO();
                break;
            case STATE.INFO_ONLY:
                appearINFOONLY();
                break;
            default:
                // something wrong, set back state
                state = currentState;
                break;

        }
    }

    private void appear(experimentaAnimationPresets thing, bool shouldBeVisible)
    {
        thing.visible = shouldBeVisible;
    }

    public void appearCOMPLETE()
    {
        appear(mapFrame, true);
        appear(meterFrame, true);
        appear(meterFrameStraight, false);
        appear(meterFrameCutout, true);
        appear(meterInstruments, true);
        appear(compassBar, true);
        appear(compassForeground, true);
        appear(compassBarShrinker, true);
        appear(compassBarMover, true);
        currentState = state = STATE.COMPLETE;
    }

    public void appearNOINFO()
    {
        appear(mapFrame, true);
        appear(meterFrame, true);
        appear(meterFrameStraight, false);
        appear(meterFrameCutout, true);
        appear(meterInstruments, true);
        appear(compassBar, true);
        appear(compassForeground, true);
        appear(compassBarShrinker, false);
        appear(compassBarMover, true);
        currentState = state = STATE.NO_INFO;
    }

    public void appearINFOONLY()
    {
        appear(mapFrame, false);
        appear(meterFrame, false);
        appear(meterFrameStraight, true);
        appear(meterFrameCutout, false);
        appear(meterInstruments, false);
        appear(compassBar, true);
        appear(compassForeground, false);
        appear(compassBarShrinker, true);
        appear(compassBarMover, false);
        currentState = state = STATE.INFO_ONLY;
    }

    public void appearHIDDEN()
    {
        appear(mapFrame, false);
        appear(meterFrame, false);
        appear(meterFrameStraight, true);
        appear(meterFrameCutout, false);
        appear(meterInstruments, false);
        appear(compassBar, false);
        appear(compassForeground, false);
        appear(compassBarShrinker, true);
        appear(compassBarMover, false);
        currentState = state = STATE.HIDDEN;
    }

    public Vector3 getHandAngle( float value, float min, float max, float minAngle, float maxAngle, float currentAngle)
    {
        float targetAngle = BenjasMath.map(value, min, max, minAngle, maxAngle);
        if (instrumentMaxDegreesPerSec > 0) targetAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, instrumentMaxDegreesPerSec * Time.deltaTime);
        
        return new Vector3(0,0, targetAngle);
    }
        
        
        public void updatespeedo()
    {
        if (glider != null)
        {
            sppedoSmoothing = Mathf.Clamp(sppedoSmoothing, 0, 0.999999f);
            if (ignoreVerticalSpeed)  speedoValue = Mathf.Lerp(glider.speedHorrizontal, speedoValue, sppedoSmoothing);
           else                     speedoValue = Mathf.Lerp(glider.speed, speedoValue, sppedoSmoothing);
            speedoHand.localEulerAngles = getHandAngle(speedoValue, speedoMin, speedoMax, speedoMinAngle, speedoMaxAngle, speedoHand.localEulerAngles.z);
            speedDigits.text = Mathf.FloorToInt(speedoValue).ToString() + " " + speedUnit;

            //adaptive smoothing depending on height
            altChangeSmoothingMinMax.x = Mathf.Clamp(altChangeSmoothingMinMax.x, 0, 0.999999f);
            altChangeSmoothingMinMax.y = Mathf.Clamp(altChangeSmoothingMinMax.y, 0, 0.999999f);
            altChangeSmoothing = BenjasMath.map(altitudeValue
                                                , altChangeSmoothingAltLimits.x, altChangeSmoothingAltLimits.y
                                                , altChangeSmoothingMinMax.x, altChangeSmoothingMinMax.y);
            //apply values
            altitudeValue = Mathf.Lerp(glider.altitude, altitudeValue, altChangeSmoothing);
            altChangeValue = Mathf.Lerp(glider.altChange, altChangeValue, altChangeSmoothing);
            altChangeHand.localEulerAngles = getHandAngle(altChangeValue, altChangeMin, altChangeMax, altChangeMinAngle, altChangeMaxAngle, altChangeHand.localEulerAngles.z);

            int alt = Mathf.FloorToInt(altitudeValue);
            string altsring = alt.ToString();
            altDigitsRed.text = altsring;
            altDigitsBlack.text = altsring;
            if (alt < 10) altsring = "0" + altsring;
            if (alt < 100) altsring = "0" + altsring;
            //if (alt < 1000) altsring = "0" + altsring;
            altDigitsWhite.text = altsring;
            altDigitsUnit.text = altUnit;
        }
    }

    public void setGameTime(float time0to1)
    {
        timePieFill.fillAmount = Mathf.Clamp01(time0to1);

    }

    public void setLevelString(string levelInfoString)
    {
        levelInfoText.text = levelInfoString;
    }


    // Update is called once per frame
    void Update () {
        updatespeedo();
        if (currentState != state) toggleVisibility();
    }
}
