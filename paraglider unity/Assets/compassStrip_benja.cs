using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class compassStrip_benja : MonoBehaviour {

    public RectTransform compassStrip;
    public RectTransform baconOne;
    public float xPosAtZero;
    public float xPosAt360;
    public float clampBaconLeft;
    public float clampBaconRight;
    public float latitudeCompass;

    // Use this for initialization
    void Start () {
		
	}




    //BUG BUG BUG !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    //only update compass after level have been loaded, ner during load


    public float xposCompass = 0;
    public void setCompass(float Angle)
    {
        Vector3 pos = compassStrip.anchoredPosition3D ;
        latitudeCompass = BenjasMath.keepAngle0to360(Angle);
        xposCompass = BenjasMath.map(latitudeCompass, 0,360,xPosAtZero,xPosAt360,false);
        pos.x = xposCompass;
        compassStrip.anchoredPosition3D  = pos;
    }

    public float latitudeBacon = 0;
    public float xposBacon = 0;

    public void setBacon(float Angle)
    {
        Vector3 pos = baconOne.anchoredPosition3D ;
        latitudeBacon = BenjasMath.keepAngle0to360(Angle);

        xposBacon = BenjasMath.map(latitudeBacon, 0, 360, xPosAtZero, xPosAt360);
        //center around 0 degrees
        if (xposBacon > Mathf.Lerp(0.5f, xPosAtZero, xPosAt360))
        {
            xposBacon -= xPosAt360 - xPosAtZero;
        }
        xposBacon = Mathf.Clamp(xposBacon, clampBaconLeft, clampBaconRight);
        pos.x = xposBacon;
        compassStrip.anchoredPosition3D  = pos;
    }


    // Update is called once per frame
    void Update () {
        setCompass(latitudeCompass);

    }
}
