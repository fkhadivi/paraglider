using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class compassStrip_benja : MonoBehaviour {

    public RectTransform compassStrip;
    public RectTransform baconOne;
    public float xPosAtZero;
    public float xPosAt360;
    public float clampBaconAt;
    public float clampBaconAngleAt;
    public float latitudeCompass;
    public float xposCompass = 0;
    public float angleBacon = 0;
    public float xposBacon = 0;

    // Use this for initialization
    void Start () {

    }




    //BUG BUG BUG !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    //only update compass after level have been loaded, ner during load




    public void setCompass(float Angle)
    {
        Vector3 pos = compassStrip.anchoredPosition3D ;
        latitudeCompass = BenjasMath.keepAngle0to360(Angle);
        xposCompass = BenjasMath.map(latitudeCompass, 0,360,xPosAtZero,xPosAt360,false);
        pos.x = xposCompass;
        compassStrip.anchoredPosition3D  = pos;
    }



    public void setBacon(float Angle)
    {
        Vector3 pos = baconOne.anchoredPosition3D ;
        angleBacon = BenjasMath.keepAngleBetween(Angle,-180, 180);

        xposBacon = BenjasMath.map(angleBacon, -clampBaconAngleAt, clampBaconAngleAt, -clampBaconAt, clampBaconAt,true);
        //center around 0 degrees
        pos.x = xposBacon;
        baconOne.anchoredPosition3D  = pos;
    }


    // Update is called once per frame
    void Update () {
        setCompass(latitudeCompass);

    }
}
