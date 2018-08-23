using UnityEngine;
using System;
using System.Xml;
using System.Globalization;

public class Configuration
{
    //private enum TimeUnit { min, sec, ms };
    static public float SCREENWIDTH             = 1920;
    static public float SCREENHEIGHT            = 1080;
    static public string head_tag               = "config";
    static public string timeout_tag            = "timeout";
    static public string timeunit_attribute     = "timeunit";

    static XmlDocument configXML;
    static string configFileName                = "config.xml";
    static bool loaded                          = false;

    static public void LoadConfig()
    {
        if (loaded)
            return;

        try
        {
            var culture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            XmlDocument xmldoc = new XmlDocument();

            //TODO: Test for UDOO
            //xmldoc.Load("file:///storage/emulated/0/Download/huettinger/" + configFileName);

            xmldoc.Load(configFileName);

            configXML = xmldoc;
            //Debug.Log("Loaded Config: " + configXML.InnerXml);
            loaded = true;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message + ". config.xml doesn't exist yet. Creating a new one ...");
        }
    }

    static private void CreateConfig()
    {
        XmlDocument xmldoc = new XmlDocument();
        XmlNode xmlRoot, xmlNode;
        XmlAttribute newAttribute;
        //XmlAttribute newAttribute;
        xmlRoot = xmldoc.CreateElement("config");
        xmldoc.AppendChild(xmlRoot);


        xmlNode = xmldoc.CreateElement(timeout_tag);
        xmlNode.InnerText = "" + 120;
        xmlRoot.AppendChild(xmlNode);
        newAttribute = xmldoc.CreateAttribute(timeunit_attribute);
        newAttribute.Value = "sec";
        xmlNode.Attributes.Append(newAttribute);

        /*
        xmlNode = xmldoc.CreateElement(moveHorizontal_tag);
        xmlNode.InnerText = "" + true;
        element.AppendChild(xmlNode);
        */
        /*
        xmlNode = xmldoc.CreateElement(maxduration);
        xmlNode.InnerText = "" + 10;
        element.AppendChild(xmlNode);
        */
        xmldoc.Save(".\\" + configFileName);
        Debug.Log(xmldoc.InnerXml);
        configXML = xmldoc;
    }

    static public void SaveSingleValue(string name, object value)
    {
        try
        {
            Configuration.configXML.GetElementsByTagName(name)[0].InnerText = value.ToString();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        configXML.Save(".\\" + configFileName);
    }

    static public string GetInnerTextByNodesName(string nodename, string defaultString = "", int count = 0)
    {
        try
        {
            if (configXML == null)
                LoadConfig();
            string[] nodes = nodename.Split('/');
            string lastnode = nodename;
            for(int i = 0; i < nodes.Length; i++)
            {
                lastnode = nodes[i];
            }
            Debug.Log("lastnode " + lastnode);
            nodename = nodename.Replace("/" + lastnode, "");
            return configXML.SelectNodes(nodename)[count][lastnode].InnerText;
        }
        catch (Exception ex)
        {
            Debug.Log("Error: Could not find Tagname: " + nodename + ". Using standard value: " + defaultString + "  Error: " + ex.Message);
            return defaultString;
        }
    }


    static public string GetInnerTextByTagName(string tagname, string defaultString = "", int count = 0)
    {
        try
        {
            if (configXML == null)
                LoadConfig();
            return configXML.GetElementsByTagName(tagname)[count].InnerText;
        }
        catch (Exception ex)
        {
            Debug.Log("Error: Could not find Tagname: " + tagname + ". Using standard value: " + defaultString + "  Error: " + ex.Message);
            return defaultString;
        }
    }

    static public double GetInnerTextByTagName(string tagname, double defaultDouble, int count = 0)
    {
        LoadConfig();
       
        string s = GetInnerTextByTagName(tagname, defaultDouble.ToString(), count);
        if (s != "")
        {
			double localCultreResult;
			double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out localCultreResult);
            Debug.Log("Config: " + tagname + " = " + localCultreResult);
            return localCultreResult;
        }
        return defaultDouble;
    }

    static public bool GetInnerTextByTagName(string tagname, bool defaultvalue, int count = 0)
    {
        string s = GetInnerTextByTagName(tagname, defaultvalue.ToString(), count);
        if (s != "")
        {
                return (Convert.ToBoolean(s));
        }
        return defaultvalue;
    }

    static public string GetAttricuteByTagName(string tagname, string attribute, int count = 0)
    {
        if (configXML == null)
            LoadConfig();

        return configXML.GetElementsByTagName(tagname)[count].Attributes[attribute].Value;
    }

    static public void Delete()
    {
        string filepath = ".\\" + configFileName;

        if (System.IO.File.Exists(filepath))
            System.IO.File.Delete(filepath);
    }

    static public void SaveValueInConfig(object innerText, string tag, int index = 0)
    {
        if (configXML == null)
            LoadConfig();

        configXML.GetElementsByTagName(tag)[index].InnerText = innerText.ToString();
        Debug.Log("Newly saved input Values: " + configXML.InnerXml);
        configXML.Save(".\\" + configFileName);
    }
}