using UnityEngine;
using System;
using System.Xml;
using System.Collections;

public class Configuration : MonoBehaviour
{
    //you can set values in Inspector only
    public enum TimeUnit { min, sec, ms };
    public string configFileName = "config.xml";

    //Root
    [NonSerialized]
    public string head_tag = "config";

    //Controller
    [NonSerialized]
    public string controller_tag            = "controller";
    [NonSerialized]
    public string maxEnginePower_tag        = "maxenginepower";
    [NonSerialized]
    public string lift_tag                  = "lift";
    [NonSerialized]
    public string zeroliftspeed_tag         = "zeroliftspeed";
    [NonSerialized]
    public string rolleffect_tag            = "rolleffect";
    [NonSerialized]
    public string pitcheffect_tag           = "pitcheffect";
    [NonSerialized]
    public string yaweffect_tag             = "yaweffect";
    [NonSerialized]
    public string bankedturneffect_tag      = "bankedturneffect";
    [NonSerialized]
    public string aerodynamicEffect_tag     = "aerodynamiceffect";
    [NonSerialized]
    public string autoTurnPitch_tag         = "autoturnpitch";
    [NonSerialized]
    public string autoRollLevel_tag         = "autorolllevel";
    [NonSerialized]
    public string autoPitchLevel_tag        = "autopitchlevel";
    [NonSerialized]
    public string airbrakeseffect_tag       = "airbrakeseffect";
    [NonSerialized]
    public string throttleChangeSpeed_tag   = "throttlechangespeed";
    [NonSerialized]
    public string dragIncreaseFactor_tag    = "dragincreasefactor";
    [NonSerialized]
    public string maxSpeed_tag           = "maxspeed";
    [NonSerialized]
    public string maxRollAngle_tag          = "maxrollangle";
    [NonSerialized]
    public string maxPitchAngle_tag         = "maxpitchangle";

    private float maxEnginePower            = 8;
    private float lift                      = 0.015f;
    private float zeroliftspeed             = 70;
    private float rolleffect                = 0.5f;
    private float pitcheffect               = 1;
    private float yaweffect                 = -0.0033f;
    private float bankedturneffect          = 5.5f;
    private float aerodynamicEffect         = 0;
    private float autoTurnPitch             = 0.5f;
    private float autoRollLevel             = 2;
    private float autoPitchLevel            = 0.1f;
    private float airbrakeseffect           = 3;
    private float throttleChangeSpeed       = 0.3f;
    private float dragIncreaseFactor        = 0.001f;
    private float maxSpeed                  = 37;
    private float maxRollAngle              = 80;
    private float maxPitchAngle             = 80;

    //Startscreen
    [NonSerialized]
    public string startscreen_tag           = "startscreen";
    [NonSerialized]
    public string xydeadzone_tag            = "xydeadzone";

    private float xydeadzone                = 0.3f;
   
    //Game
    [NonSerialized]
    public string game_tag                   = "game";
    [NonSerialized]
    public string timeout_tag                = "timeout";
    [NonSerialized]
    public string countdown_tag              = "countdown";
    private string timeunit_attribute        = "timeunit";
    private float timeout_game               = 120;
    private float countdownNum               = 10;
    [NonSerialized]
    public string crashscreen_timeout_tag = "crashscreentimeout";
    private float crash_timeout = 1;
    //crashscreen_timeout

    //Input
    [NonSerialized]
    public string input_tag = "input";
    [NonSerialized]
    public string autosave_tag = "autosave";
    [NonSerialized]
    public string xdeadzone_tag = "xdeadzone";
    [NonSerialized]
    public string ydeadzone_tag = "ydeadzone";
    [NonSerialized]
    public string centerXAxis_tag = "centerxaxis";
    [NonSerialized]
    public string centerYAxis_tag = "centeryaxis";
    [NonSerialized]
    public string minXAxis_tag = "minxaxis";
    [NonSerialized]
    public string maxXAxis_tag = "maxxaxis";
    [NonSerialized]
    public string minYAxis_tag = "minyaxis";
    [NonSerialized]
    public string maxYAxis_tag = "maxyaxis";
    [NonSerialized]
    public string invertX_tag = "invertX";
    [NonSerialized]
    public string invertY_tag = "invertY";

    //RFID
    [NonSerialized]
    public string rfid_tag = "rfid";
    [NonSerialized]
    public string comport_tag = "comport";
    [NonSerialized]
    public string baurate_tag = "baurate";
    [NonSerialized]
    public string parity_tag = "parity";
    [NonSerialized]
    public string stopBits_tag = "stopbits";
    [NonSerialized]
    public string dataBits_tag = "databits";

    [NonSerialized]
    public string comportname = "COM3";
    [NonSerialized]
    public string baurate = "9600";
    [NonSerialized]
    public string parity = "none";
    [NonSerialized]
    public string stopBits = "One";
    [NonSerialized]
    public string dataBits = "8";
    
    public bool autoSave = true;
    private float xdeadzone = 0.01f, ydeadzone_value = 0.05f;
    private float XaxesInputVal, YaxesInputVal;
    private float centerXAxis = 0, centerYAxis = 0;
    private float minXAxis = 100, maxXAxis = -100;
    private float minYAxis = 100, maxYAxis = -100;
    private float minXAxisWithoutDeadzone, maxXAxisWithoutDeadzone;
    private float minYAxisWithoutDeadzone, maxYAxisWithoutDeadzone;
    private float xRangeWithoutDeadzone, yRangeWithoutDeadzone;
    private bool  invertX, invertY;

    private XmlDocument configXML;

    public void Awake()
    {
        LoadConfig();
    }

    private void LoadConfig()
    {
        try
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(".\\" + configFileName);
            autoSave    = Convert.ToBoolean(xmldoc.GetElementsByTagName(autosave_tag)[0].InnerText.ToLower());
            xdeadzone   = (float)Convert.ToDouble(xmldoc.GetElementsByTagName(xdeadzone_tag  )[0].InnerText);
            ydeadzone_value   = (float)Convert.ToDouble(xmldoc.GetElementsByTagName(ydeadzone_tag  )[0].InnerText);
            centerXAxis      = (float)Convert.ToDouble(xmldoc.GetElementsByTagName(centerXAxis_tag)[0].InnerText);
            centerYAxis      = (float)Convert.ToDouble(xmldoc.GetElementsByTagName(centerYAxis_tag)[0].InnerText);
            minXAxis         = (float)Convert.ToDouble(xmldoc.GetElementsByTagName(minXAxis_tag   )[0].InnerText);
            maxXAxis         = (float)Convert.ToDouble(xmldoc.GetElementsByTagName(maxXAxis_tag   )[0].InnerText);
            minYAxis         = (float)Convert.ToDouble(xmldoc.GetElementsByTagName(minYAxis_tag   )[0].InnerText);
            maxYAxis         = (float)Convert.ToDouble(xmldoc.GetElementsByTagName(maxYAxis_tag   )[0].InnerText);
            invertX          = Convert.ToBoolean(xmldoc.GetElementsByTagName(invertX_tag)[0].InnerText.ToLower());
            invertY          = Convert.ToBoolean(xmldoc.GetElementsByTagName(invertY_tag)[0].InnerText.ToLower());

            configXML   = xmldoc;
            UnityEngine.Debug.Log("Loaded Config: " + configXML.InnerXml);

            CalculateRanges();
        }
        catch (Exception e)
        {
            //if loading isn't successful
            // create a new config file
            UnityEngine.Debug.Log(e.Message + ". config.xml doesn't exist yet. Creating a new one ...");
            CreateConfig();
        }
    }
    /*
    void Start()
    {
        if (autoSave)
        {
            SaveJoystickCenter();
        }

    }
    */
    public void UpdateAxesInputs(ref float X_value, ref float Y_value, bool joystickIsUsed)
    {
        if (!joystickIsUsed)
            return;

        bool saveFlag = false;
        float rawXInput = X_value; //Input.GetAxis("Horizontal");
        float rawYInput = Y_value; //Input.GetAxis("Throttle");

        if (autoSave)
        {
            // checking min and max and save it if neccessary
            if (rawXInput < minXAxis && rawXInput != 0)
            {
                minXAxis = rawXInput;
                saveFlag = true;
            }
            else if (rawXInput > maxXAxis && rawXInput != 0)
            {
                maxXAxis = rawXInput;
                saveFlag = true;
            }
            if (rawYInput < minYAxis && rawYInput != 0)
            {
                minYAxis = rawYInput;
                saveFlag = true;
            }
            else if (rawYInput > maxYAxis && rawYInput != 0)
            {
                maxYAxis = rawYInput;
                saveFlag = true;
            }

            if (saveFlag) SaveExtremaAxisInConfigXml();
        }

        // calculate the new inputs 
        // X Axes -------------------------------------------------
        XaxesInputVal = X_value;
        if (Math.Abs(X_value - centerXAxis) < xdeadzone)
        {
            XaxesInputVal = 0;
        }
        else
        {
            if (X_value  > centerXAxis)
            {
                XaxesInputVal = XaxesInputVal - centerXAxis - xdeadzone;
                XaxesInputVal = XaxesInputVal / (maxXAxis - centerXAxis - xdeadzone);
            }
            else
            {
                // negative side of center
                XaxesInputVal = centerXAxis - XaxesInputVal - xdeadzone;
                XaxesInputVal = XaxesInputVal / (centerXAxis - minXAxis - xdeadzone);  // now ranging from 0 to 1
                XaxesInputVal = XaxesInputVal * -1;                                         // now ranging from 0 to -1
            }
        }

        // Y Axes  - ------------------------------------------------
        YaxesInputVal = Y_value ; // center value

        if (Math.Abs(Y_value - centerYAxis) < ydeadzone_value)
        {
            YaxesInputVal = 0;
        }
        else
        {
            if (Y_value > centerYAxis)
            {
                YaxesInputVal = YaxesInputVal - centerYAxis - ydeadzone_value;
                YaxesInputVal = YaxesInputVal / (maxYAxis - centerYAxis - ydeadzone_value);
            }
            else
            {
                // negative side of center
                YaxesInputVal = centerYAxis - YaxesInputVal - ydeadzone_value;
                YaxesInputVal = YaxesInputVal / (centerYAxis - minYAxis - ydeadzone_value);  // now ranging from 0 to 1
                YaxesInputVal = YaxesInputVal * -1;                                         // now ranging from 0 to -1
            }
        }

        X_value = (invertX) ? XaxesInputVal *= -1.0f : XaxesInputVal;
        Y_value = (invertY) ? YaxesInputVal *= -1.0f : YaxesInputVal;
    }

    private void SaveExtremaAxisInConfigXml()
    {
        configXML.GetElementsByTagName(minXAxis_tag)[0].InnerText = Convert.ToString(minXAxis);
        configXML.GetElementsByTagName(maxXAxis_tag)[0].InnerText = Convert.ToString(maxXAxis);
        configXML.GetElementsByTagName(minYAxis_tag)[0].InnerText = Convert.ToString(minYAxis);
        configXML.GetElementsByTagName(maxYAxis_tag)[0].InnerText = Convert.ToString(maxYAxis);

        UnityEngine.Debug.Log("Newly saved input Values: " + configXML.InnerXml);
        configXML.Save(".\\" + configFileName);

    }

    private void CreateConfig()
    {
        XmlDocument xmldoc = new XmlDocument();
        //XmlComment comm = new XmlComment();
        XmlNode xmlRoot, element, xmlNode, xmlnodechild;
        XmlAttribute newAttribute;
        xmlRoot = xmldoc.CreateElement(head_tag);
        xmldoc.AppendChild(xmlRoot);

        //Game
        element = xmldoc.CreateElement(game_tag);
        xmlRoot.AppendChild(element);

        xmlNode = xmldoc.CreateElement(timeout_tag);
        xmlNode.InnerText = "" + 120;
        element.AppendChild(xmlNode);
        newAttribute = xmldoc.CreateAttribute(timeunit_attribute);
        newAttribute.Value = "sec";
        xmlNode.Attributes.Append(newAttribute);

        xmlNode = xmldoc.CreateElement(countdown_tag);
        xmlNode.InnerText = "" + 3;
        element.AppendChild(xmlNode);
        newAttribute = xmldoc.CreateAttribute(timeunit_attribute);
        newAttribute.Value = "sec";
        xmlNode.Attributes.Append(newAttribute);

        xmlNode = xmldoc.CreateElement(crashscreen_timeout_tag);
        xmlNode.InnerText = "" + crash_timeout;
        element.AppendChild(xmlNode);
        newAttribute = xmldoc.CreateAttribute(timeunit_attribute);
        newAttribute.Value = "sec";
        xmlNode.Attributes.Append(newAttribute);

        //Controller
        element = xmldoc.CreateElement(controller_tag);
        xmlRoot.AppendChild(element);

        xmlNode = xmldoc.CreateElement(maxEnginePower_tag);
        xmlNode.InnerText = "" + 8;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(lift_tag);
        xmlNode.InnerText = "" + 0.015f;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(zeroliftspeed_tag);
        xmlNode.InnerText = "" + 70;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(rolleffect_tag);
        xmlNode.InnerText = "" + 0.5f;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(pitcheffect_tag);
        xmlNode.InnerText = ""+ 1;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(yaweffect_tag);
        xmlNode.InnerText = ""+ (-0.0033f);
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(bankedturneffect_tag);
        xmlNode.InnerText = ""+ 5.5f;
        element.AppendChild(xmlNode);

        /*
        xmlNode = xmldoc.CreateElement(aerodynamicEffect_tag);
        xmlNode.InnerText = ""+ 0.03f;
        element.AppendChild(xmlNode);
        */

        xmlNode = xmldoc.CreateElement(autoTurnPitch_tag);
        xmlNode.InnerText = ""+ 0.5f;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(autoRollLevel_tag);
        xmlNode.InnerText = ""+ 2.0f;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(autoPitchLevel_tag);
        xmlNode.InnerText = "" + 0.1f;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(airbrakeseffect_tag);
        xmlNode.InnerText = "" + 3.0f;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(throttleChangeSpeed_tag);
        xmlNode.InnerText = "" + 0.3f;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(dragIncreaseFactor_tag);
        xmlNode.InnerText = "" + 0.001f;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(maxRollAngle_tag);
        xmlNode.InnerText = "" + 80;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(maxPitchAngle_tag);
        xmlNode.InnerText = "" + 80;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(maxSpeed_tag);
        xmlNode.InnerText = "" + maxSpeed;
        element.AppendChild(xmlNode);

        //Input
        element = xmldoc.CreateElement(input_tag);
        xmlRoot.AppendChild(element);

        xmlNode = xmldoc.CreateElement(autosave_tag);
        xmlNode.InnerText = ""+autoSave;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(centerXAxis_tag);
        xmlNode.InnerText = ""+centerXAxis;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(minXAxis_tag);
        xmlNode.InnerText = ""+minXAxis;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(maxXAxis_tag);
        xmlNode.InnerText = ""+maxXAxis;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(xdeadzone_tag);
        xmlNode.InnerText = Convert.ToString(xdeadzone);
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(invertX_tag);
        xmlNode.InnerText = "" + invertX;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(centerYAxis_tag);
        xmlNode.InnerText = "" + centerYAxis;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(minYAxis_tag);
        xmlNode.InnerText = ""+ minYAxis;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(maxYAxis_tag);
        xmlNode.InnerText = ""+ maxYAxis;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(ydeadzone_tag);
        xmlNode.InnerText = Convert.ToString(ydeadzone_value);
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(invertY_tag);
        xmlNode.InnerText = "" + invertY;
        element.AppendChild(xmlNode);

        //rfid
        element = xmldoc.CreateElement(rfid_tag);
        xmlRoot.AppendChild(element);

        xmlNode = xmldoc.CreateElement(comport_tag);
        xmlNode.InnerText = comportname;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(baurate_tag);
        xmlNode.InnerText = baurate;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(parity_tag);
        xmlNode.InnerText = parity;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(stopBits_tag);
        xmlNode.InnerText = stopBits;
        element.AppendChild(xmlNode);

        xmlNode = xmldoc.CreateElement(dataBits_tag);
        xmlNode.InnerText = dataBits;
        element.AppendChild(xmlNode);

        xmldoc.Save(".\\" + configFileName);
        //UnityEngine.Debug.Log(xmldoc.InnerXml);
        configXML = xmldoc;
        configXML.Save(".\\" + configFileName);
    }

    public void SaveJoystickCenter(float x_value, float y_value)
    {
        if (!autoSave) return;
        centerXAxis = x_value;
        configXML.GetElementsByTagName(centerXAxis_tag)[0].InnerText = Convert.ToString(centerXAxis);
        centerYAxis = y_value;
        configXML.GetElementsByTagName(centerYAxis_tag)[0].InnerText = Convert.ToString(centerYAxis);
        configXML.Save(".\\" + configFileName);
    }

    private void CalculateRanges()
    {
        xRangeWithoutDeadzone = (maxXAxis - minXAxis)  - 2*xdeadzone;
        yRangeWithoutDeadzone = (maxYAxis - minYAxis)  - 2*ydeadzone_value;

        minXAxisWithoutDeadzone += xdeadzone;
        maxXAxisWithoutDeadzone -= xdeadzone;
        minYAxisWithoutDeadzone += ydeadzone_value;
        maxYAxisWithoutDeadzone -= ydeadzone_value;
    }

    public string GetElementByTagNameInnerXml(string tagname, int count)
    {
        //Debug.Log("Tagname: " + tagname);
        return configXML.GetElementsByTagName(tagname)[count].InnerText;
    }

    public string GetInnerTextOfSelectedNodes(string tagname, int count)
    {
        //Debug.Log(" Tagename: " + tagname);
        XmlNodeList xnList = configXML.SelectNodes(tagname);
        return xnList[count].InnerText;
    }

    public string GetAttributeOfSelectedNodes(string tagname, int count, string attributeName)
    {
        XmlNodeList xnList = configXML.SelectNodes(tagname);
        return xnList[count].Attributes[attributeName].Value;
    }

    public string GetElementByTagNameValue(string tagname, int count, string attribute)
    {
        return configXML.GetElementsByTagName(tagname)[count].Attributes[attribute].Value;
    }

    public void ResetCalibration(float x, float y)
    {
        autoSave = true;
        SaveJoystickCenter(x, y);
        minXAxis = 100;
        maxXAxis = -100;
        minYAxis = 100;
        maxYAxis = -100;
    }

    public void DisableAutoSave(bool disable = true)
    {
        autoSave = !disable;
        configXML.GetElementsByTagName(autosave_tag)[0].InnerText = Convert.ToString(autoSave);
        configXML.Save(".\\" + configFileName);
    }

    public void Delete()
    {
        string filepath = ".\\" + configFileName;

        if (System.IO.File.Exists(filepath))
            System.IO.File.Delete(filepath);
    }
}