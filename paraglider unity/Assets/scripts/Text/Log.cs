using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Log {

    public static List<string> txtsForConsole = new List<string>();
    public static bool messageIsNew = false;
    static string logTextForUI = "";

    public static bool extendedLogging = true;

    public static void Init()
    {
        Application.logMessageReceived += HandleLog;

        string logFile;

#if !UNITY_ANDROID || UNITY_EDITOR
        if (extendedLogging)
        {
            if(!Directory.Exists("logs")) Directory.CreateDirectory("logs");
            logFile = ".\\logs\\log_" + DateTime.Now.ToString("yyyy-MM-dd HH-mm") + ".txt";
        }
        else
        {
            logFile = "log.txt";
        }

        try
        {
            FileStream fs = new FileStream(logFile, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            sw.AutoFlush = true;
            Console.SetOut(sw);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("Error while opening log file '" + logFile + "'.");
            Exception(ex);
        }

#elif UNITY_ANDROID && !UNITY_EDITOR
        if (extendedLogging)
        {
            if(!Directory.Exists(IGP.Resources.folderPathForAndroid+ "/logs")) 
                Directory.CreateDirectory(IGP.Resources.folderPathForAndroid+ "/logs");
        
            logFile = IGP.Resources.folderPathForAndroid+ "/logs/log_" + DateTime.Now.ToString("yyyy-MM-dd HH-mm") + ".txt";
        }
        else
        {
            logFile = IGP.Resources.folderPathForAndroid+ "/log.txt";
        }

        try
        {
            FileStream fs = new FileStream(logFile, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            sw.AutoFlush = true;
            Console.SetOut(sw);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("Error while opening log file '" + logFile + "'.");
            Exception(ex);
        }
#endif

    }

    public static void HandleLog(string logString, string stackTrace, LogType type )
    {
        Message(logString, false, false, type);
    }

    public static void SetExtendedLogging()
    {
        extendedLogging = true;
        Init();
    }

    

    static public void Message(object message, bool hideDate = false, bool NoNewLine = false, LogType type = LogType.Log)
    {
        messageIsNew = true;
        message = (!hideDate == true ? " " + DateTime.Now.ToString("hh:mm:ss.fff") + " :  " + message : message);

        Console.WriteLine(message);

        //text length must be limited for UI
        string newLineString = (!NoNewLine) ? Environment.NewLine : " ";
        txtsForConsole.Add(message + newLineString);
    }

    static public void Debug(string message, bool hideDate = false, bool noNewLine = false)
    {
        message = "<color=#0084ffff>" + "DEBUG: " + "</color>" + message;
        Message(message,  hideDate, noNewLine, LogType.Log);
    }

    static public void Error(string message, bool hideDate = false, bool noNewLine = false)
    {
        message = "<color=#ff0000ff>" + "ERROR: " + "</color>" + message;
        Message(message, hideDate, noNewLine, LogType.Error);
    }

    static public void Warning(string message, bool hideDate = false, bool noNewLine = false)
    {
        message = "<color=#ffcc00ff>" + "WARNING: " + "</color>" + message;
        Message(message, hideDate, noNewLine, LogType.Warning);
    }

    static public void Exception(Exception e, bool hideDate = false, bool noNewLine = false)
    {
        string message = "";

        message += "****************************** EXCEPTION **************************************" + Environment.NewLine;
        message += "<color=#ff0000ff>" + e.Message + "</color>" + Environment.NewLine;
        message += "*******************************************************************************" + Environment.NewLine;

        Message(message, hideDate, noNewLine, LogType.Exception);
    }

    static public void CommandOut(string message, bool hideDate = false, bool noNewLine = false)
    {
        message = "<color=#ff00ffff>" + "COMMANDOUT: " + "</color>" + message;
        Message(message, hideDate, noNewLine, LogType.Log);
    }
}
