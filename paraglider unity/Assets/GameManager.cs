using IGP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    UDPListener udpListener;
    string ip = "127.0.0.1";
    int listnerPort = 5000;
    int senderPort = 6000;
    string languageCode = "en";

    Dictionary<string, string> messageDict = new Dictionary<string, string>();

    enum STATES
    {
        SCREENSAVER,
        INSTRUCTION,
        COUNTDOWN,
        PLAYING,
        GAMEOVER,
        RESULT,
        PAUSE
    }

    public enum GameOverTypes
    {
        WIN,
        LOSE,
        TIMEOUT
    }

    // Use this for initialization

    void Start () {

        listnerPort = (int)Configuration.GetInnerTextByTagName("listnerPort", listnerPort);
        ip = Configuration.GetInnerTextByTagName("ip", ip);

        udpListener = new UDPListener();
        udpListener.MessageReceived += OnMessageReceived;
        udpListener.Start(listnerPort);

        bool loaded = TextProvider.Load("text_9681.xlsx");
        if (!loaded)
        {
            loaded = TextProvider.Load("text.xlsx");
			if (!loaded)
			{
				TextProvider.Load("text.csv");
			}
        }

        TextProvider.lang = languageCode;
    }

    void OnMessageReceived(object sender, string message)
    {
        //Debug.Log("message " + message);
        messageDict.Clear();
        string[] aKeysValues = message.Split(';');

        for (var i = 0; i < aKeysValues.Length; i++)
        {
            var aKeyValuePairs = aKeysValues[i].Split('=');
            messageDict[aKeyValuePairs[0]] = aKeyValuePairs[1];
        }

        if(messageDict["countdown"] != null)
        {
            string curNum = messageDict["countdown"];
        }
    }

    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("Text: " + TextProvider.GetText("0404.MT.0100.HL0"));
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            UDPSender.SendUDPStringUTF8(ip,senderPort,"Hello World!!!");
        }
	}

    public void GameStart()
    {
        UDPSender.SendUDPStringUTF8(ip, senderPort, "state=game,action=start");
    }

    public void OnPause()
    {
        UDPSender.SendUDPStringUTF8(ip, senderPort, "state=game,action=pause");
    }

    public void GameOver(bool isPlayerDeath )
    {
        string action = "timeout";

        if (isPlayerDeath)
        {
            action = "lose";
        }

        UDPSender.SendUDPStringUTF8(ip, senderPort, "state=gameover,action=" + action);
    }

    public void GameFinished()
    {
        UDPSender.SendUDPStringUTF8(ip, senderPort, "state=gamefinished,action=win");
    }

    public void ChangeLanguage()
    {
        if(languageCode == "en") {
            languageCode = "ar";
        }
        else
        {
            languageCode = "en";
        }

        TextProvider.lang = languageCode;
        UDPSender.SendUDPStringUTF8(ip, senderPort, "language=" + languageCode);
    }
}
