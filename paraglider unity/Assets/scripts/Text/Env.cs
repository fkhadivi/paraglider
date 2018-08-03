using System;
using UnityEngine;
using System.IO;
using Microsoft.Win32;


/// <summary>
/// Environment class. Handles to global context of the application.
/// </summary>
public class Env
{
    /// <summary> Command line script file. </summary>
    public static string commandLineScript = null;

    /// <summary> Config file for local settings. </summary>
    public static IniFile ini;

    /// <summary> Base folder for file/resource access. </summary>
    public static string baseFolder = Directory.GetCurrentDirectory();

    /// <summary> Load environment settings. </summary>
    public static bool Load(string fileName)
    {
        if (File.Exists(fileName))
        {
            Debug.Log("Loading environment from '" + fileName + "'.");
            ini = new IniFile(fileName);
            return true;
        }else
        {
            Debug.Log("can't find environment from '" + fileName + "'.");
        }
        return false;
    }


    public static bool HasSetting(string key)
    {
        if (ini == null) return false;
        try
        {
            return ini.CheckKey("config", key);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static void SetSetting(string key, object value)
    {
        if (ini == null) return;
        //if (GetSetting(key) == value.ToString()) return;
        ini.WriteValue("config", key, value.ToString());
    }

    public static string GetSetting(string key, string _default = "")
    {
        if (ini == null) return _default;
        string value = ini.ReadValue("config", key);
        if (!String.IsNullOrEmpty(value))
            return value;
        else
        {
            SetSetting(key, _default);
            return _default;
        }
    }

    public static int GetSettingInt(string key, int _default = 0)
    {
        if (ini == null) return _default;
        string value = GetSetting(key, _default.ToString());
        return Convert.ToInt32(value);
    }

    public static double GetSettingDouble(string key, double _default = 0.0)
    {
        if (ini == null) return _default;
        string value = GetSetting(key, _default.ToString());
        return Convert.ToDouble(value);
    }


    public static string GetApplicationPath()
    {
#if UNITY_EDITOR
        return Application.dataPath ;
#endif
        return Application.absoluteURL;
    }

    /// <summary> Gets the full, environment specific, file name. Example: "test.txt" may translate to "C:\Users\halem\AppData\Roaming\test.txt" </summary>
    public static string GetFullFileName(string file, bool logError = true)
    {
        if (file == null) return null;
        file = file.Replace("/", "\\");
        string f = Path.Combine(GetBaseFolder(), file);
        f = Path.GetFullPath(f);

        if (!System.IO.File.Exists(f))
        {
            // TODO: Look at other locations...!
            if (System.IO.File.Exists(file)) return Path.GetFullPath(file).Replace('\\', '/');

            if (logError)
                Log.Error("File \"" + f + "\" does not exist!");

            return null;
        }

        return Path.GetFullPath(f).Replace('\\', '/');
    }

    /// <summary> Get environment base folder. </summary>
    public static string GetBaseFolder()
    {
        return baseFolder;
    }
}
