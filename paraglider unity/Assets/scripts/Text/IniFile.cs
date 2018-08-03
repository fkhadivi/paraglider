using System;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IniFile : ITextFile
    {
    private Dictionary<string, Dictionary<string, string>> Ini = new Dictionary<string, Dictionary<string, string>>();
    public string Path;

    public IniFile(string path)
    {
        this.Path = path;
        ReadIni();
    }

    private bool ReadIni()
    {
        if (File.Exists(Path))
        { 
            using (StreamReader sr = new StreamReader(Path))
            {
                string line;
                string theSection = "";
                string theKey = "";
                string theValue = "";
                while (!sr.EndOfStream) // baraye inke hame safhe ro bekhoone
                {
                    line = sr.ReadLine();
                    //Debug.Log(line);
                    line.Trim();
                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        theSection = line.Substring(1, line.Length - 2); // save in tehSection
                        try
                        {
                            Ini.Add(theSection, new Dictionary<string, string>());
                        }
                        catch (Exception ex)
                        {
                            UnityEngine.Debug.LogWarning(ex.Message);
                        }
                    }
                    else if (line.Contains("="))
                    {
                        string[] keys = line.Split(new char[] { '=' });
                        theKey = keys[0].Trim();
                        theValue = "";
                        //theValue = keys[1].Trim();                     
                        for (int i = 1; i < keys.Length; i++)
                        {
                            theValue += keys[i].Trim();
                            if (i < keys.Length - 1)
                                theValue += "=";
                        }
                        try
                        {
                            Ini[theSection].Add(theKey, theValue);
                        }
                        catch (Exception ex)
                        {
                            Ini[theSection][theKey] = theValue;
                            UnityEngine.Debug.LogWarning(ex.Message);
                        }
                    }
                }
            }
            return true;
        }
        return false;
    }

    /*public string IniRead(string _section, string _key)
    {
        if (_section == null)
        {
            _section = Ini.ElementAt(0).Key;
        }

        return true;
    }*/

    void WriteIni()
    {
        using (StreamWriter sw = new StreamWriter(Path))
        {
            foreach (KeyValuePair<string, Dictionary<string, string>> iniSec in Ini)
            {
                sw.WriteLine("[" + iniSec.Key.ToString() + "]");
                foreach (KeyValuePair<string, string> section in iniSec.Value)
                {
                    string seckey = section.Key.ToString();
                    string secvalue = section.Value.ToString();
                    sw.WriteLine("{0} = {1}", seckey, secvalue);
                }
            }
            sw.Close();
        }
    }
    void WriteIni2()
    {
        string s = "";
        foreach (KeyValuePair<string, Dictionary<string, string>> iniSec in Ini)
        {
            s += "[" + iniSec.Key.ToString() + "]\n";
            foreach (KeyValuePair<string, string> section in iniSec.Value)
            {
                string seckey = section.Key.ToString();
                string secvalue = section.Value.ToString();
                s += seckey + " = " + secvalue + "\n";
            }
        }
        //Log.Message(" Saving config ini: " + s);
        System.IO.File.WriteAllText(Path, s);
    }

    public string IniReadValue(string _section, string _key)
    {
        if (_section == null)
        {
            _section = Ini.ElementAt(0).Key; // baraye inke betonim parametre null bedim
        }
        if (Ini.ContainsKey(_section))
            if (Ini[_section].ContainsKey(_key))
            {
                string ent = Ini[_section][_key];
                if (ent.Length == 0) return "";
                if (ent[0] == '"') return ent.Substring(1, ent.Length - 2);
                else return ent;
            }
        return null;
    }

    public void IniWriteValue(string _section, string _key, string _value)
    {
        if (Ini.Keys.Contains(_section))
        {
            if (Ini[_section].Keys.Contains(_key))
                Ini[_section][_key] = _value;
            else
            {
                Ini[_section].Add(_key, _value);
            }
        }
        else
        {
            Dictionary<string, string> newsec = new Dictionary<string, string>();
            newsec.Add(_key, _value);
            Ini.Add(_section, newsec);
        }
        WriteIni2();
    }

    public string ReadValue(string section, string key)
    {
        return IniReadValue(section, key);
    }
    public void WriteValue(string section, string key, string value)
    {
        IniWriteValue(section, key, value);
    }
    public bool CheckKey(string _section, string _key)
    {
        if (IniReadValue(_section, _key) != "")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}



