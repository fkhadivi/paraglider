﻿using IGP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    InputManager inputManager;
    UDPListener listener;
    static GameManager instance;

    static string ip = "127.0.0.1";
    static int port = 6000;
    int listenerPort = 5000;

    static string languageCode = "de";

    bool pulledReipcord = false;
    bool controlledGrips = false;
    bool leavedGrips = false;

    static string endtime = "";
    bool goToStandbyModusNow = false;

    public float delayStartPlaying = 1.0f; //in sec

    public static float maxTimeUntilStandby = 15.0f;    //time until switched to standby mode 

    public static STATE state;
    private static float timeUntilStandby;

    public enum STATE
    {
        STANDBYMODUS,
        INTRO,
        GAME,
        ABORT,   
        INACTIVITY,
        RESULT,
        TIMEOUT,
        GAMEOVER
    }

    public static GameManager GetInstance()
    {
        return instance;
    } 

    void Awake()
    {
        instance = this;
        Configuration.LoadConfig();
    }

    
    void Start () {
        ip           = Configuration.GetInnerTextByTagName("ip", ip);
        port         = (int)Configuration.GetInnerTextByTagName("senderPort", port);
        listenerPort = (int)Configuration.GetInnerTextByTagName("listenerPort", listenerPort);

        maxTimeUntilStandby = (float)Configuration.GetInnerTextByTagName("maxTimeUntilStandby ", maxTimeUntilStandby );

        delayStartPlaying = (float)Configuration.GetInnerTextByTagName("delayStartPlaying ", delayStartPlaying);

        listener = new UDPListener();
        listener.MessageReceived += OnMessage;

        listener.Start(listenerPort);

        inputManager = InputManager.GetInstance();

        TextProvider.Load("text_9681.xlsx");
        TextProvider.lang = languageCode;

        StartStandbymodus(); 
    }

    // Update is called once per frame
    void Update () {
        // Test --------------------------
        if (Input.GetKeyDown(KeyCode.M))
        {
            UDPSender.SendUDPStringUTF8(ip,port,"Hello World!");
        }
        // --------------------------------

        if (goToStandbyModusNow)
        {
            goToStandbyModusNow = false;           
            StartStandbymodus();
        }
    }

    public void resetStandbyTimer()
    {
        //timeUntilStandby = maxTimeUntilStandby;
    }

    private void OnMessage(object sender, string e)
    {
        Debug.Log("Message: " + e);
        if(e == "standbymodus")
        {
            goToStandbyModusNow = true;
        }
    }

    //InputManager calls the static functions
    public static void CallInactivity()
    {
        if (state != STATE.INACTIVITY)
        {
            OpenInactivityDialog();
        }
    }

    public static void CallPulledGrips()
    {
        Debug.Log("Pulled grips " + state);
        if (state == STATE.INTRO)
        {
            StartGame();
        }
        else if (state == STATE.ABORT || state == STATE.INACTIVITY)
        {
            ResumeGame();
        }    
    }

    public static void CallPulledRipcord()
    {
        Debug.Log("Pulled ripcord " + state);

        if (state == STATE.STANDBYMODUS)
        {
            GoToIntro();
        }
        else if (state == STATE.INTRO)
        {
            ChangeLanguage();
        }
        else if (state == STATE.GAME)
        {
            OpenAbort();
        }
        else if (state == STATE.ABORT)
        {
            GoToIntro();
        }

        else if (state == STATE.INACTIVITY)
        {
            StartStandbymodus();
        }
    }

    ///
    /// See 180524_exp_ie_0404_fsb_paraglider_animkom_jn.pdf
    ///
    // 1.00 Standby Modus
    public static void StartStandbymodus()
    {
        state = STATE.STANDBYMODUS;
        UDPSender.SendUDPStringUTF8(ip, port, "state=standbymodus");
        timeUntilStandby = maxTimeUntilStandby;
        if (ParagliderMainScript.GetInstance() != null)
        {
            if (ParagliderMainScript.GetInstance().state != ParagliderMainScript.STATE.RESETTING &&
                   ParagliderMainScript.GetInstance().state != ParagliderMainScript.STATE.READY)
            {
                ParagliderMainScript.GetInstance().gameReset();
            }
        }
    }

    // 2.00 Aktivierung
    public static void GoToIntro()
    {
        state = STATE.INTRO;
        UDPSender.SendUDPStringUTF8(ip, port, "state=activation;action=open;");

        if (ParagliderMainScript.GetInstance().state != ParagliderMainScript.STATE.RESETTING &&
       ParagliderMainScript.GetInstance().state != ParagliderMainScript.STATE.READY)
        {
            ParagliderMainScript.GetInstance().gameReset();
        }
    }

    // 2.00 Aktivierung
    public static void ChangeLanguage()
    {
        if(languageCode == "en") {
            languageCode = "de";
        }
        else
        {
            languageCode = "en";
        }

        TextProvider.lang = languageCode;
        UDPSender.SendUDPStringUTF8(ip, port, "state=activation;action=ripcord;value=" + languageCode);
    }

    public static void StartGame()
    {
        state = STATE.GAME;
        GameManager.GetInstance().StartCoroutine(GameManager.GetInstance().WaitForGameReady() );
    }

    IEnumerator WaitForGameReady()
    {
        Debug.Log("WaitForGameReady");
        if  (ParagliderMainScript.GetInstance().state != ParagliderMainScript.STATE.RESETTING &&
            ParagliderMainScript.GetInstance().state != ParagliderMainScript.STATE.READY)
        {
            ParagliderMainScript.GetInstance().gameReset();
        }

        //Todo: start game and turn on character control to be able to play
        while (ParagliderMainScript.GetInstance().state != ParagliderMainScript.STATE.READY)
        {
            Debug.Log("Waiting For GameReady");

            yield return new WaitForSeconds(0.1f);
        }

        UDPSender.SendUDPStringUTF8(ip, port, "state=game;action=start;");
        ParagliderMainScript.GetInstance().gameStart();

        instance.Invoke("DelayGameStartPlaying", instance.delayStartPlaying);
    }

    public void DelayGameStartPlaying()
    {
        Debug.Log("Delayed Game Start Playing");
        ParagliderMainScript.GetInstance().gameStartPlaying();
    }

    // 3.00: ID = 1
    // 4.00: ID = 2
    // 4.01: ID = 0
    // 4.02: ID = 0
    // 5.00: ID = 0
    // 6.00: ID = 4 
    public static void ChangePromptTextInGame(int _id)
    {
        state = STATE.GAME;
        string actionString = "";

        switch (_id)
        {
            case 0:
                actionString = "hideheadline";
                break;
            case 1:
                actionString = "gamestart";
                break;
            case 2:
                actionString = "control";
                break;
            case 3:
                actionString = "obstacle";
                break;
            case 4:
                actionString = "finish";
                break;
        }

        UDPSender.SendUDPStringUTF8(ip, port, "state=game;action=" + actionString);
    }

    // 7.0 Spielende
    public static void SetScore(string time)
    {
        endtime = time;
    }

    // 7.0 Spielende
    public static void GoToResult()
    {
        state = STATE.RESULT;
        UDPSender.SendUDPStringUTF8(ip, port, "state=result;action=time;score=" + endtime);
    }

    // 8.0 Unfall / Game Over 
    // 9.0 Zeit abgelaufen
    public static void GameOver(bool isTimeout)
    {
        state = STATE.GAMEOVER;
        string action = "crash";
        if (isTimeout)
        {
            action = "timeout";
        }

        UDPSender.SendUDPStringUTF8(ip, port, "state=gameover;action="+ action);
    }

    // 10.0 Abbruch
    public static void OpenAbort()
    {
        state = STATE.ABORT;
        UDPSender.SendUDPStringUTF8(ip, port, "state=abort;action=open");
        ParagliderMainScript.GetInstance().gamePause(true);
    }

    // 10.0 Abbruch
    public static void ResumeGame()
    {
        state = STATE.GAME;
        UDPSender.SendUDPStringUTF8(ip, port, "state=game;action=resume");
        ParagliderMainScript.GetInstance().gamePause(false);
    }

    // 11.0 Inaktivität
    public static void OpenInactivityDialog()
    {
        state = STATE.INACTIVITY;
        UDPSender.SendUDPStringUTF8(ip, port, "state=inactivity;action=open");
        ParagliderMainScript.GetInstance().gamePause(true);
    }

    void OnDestroy()
    {
#if UNITY_EDITOR
        OnApplicationQuit();
#endif
    }

    void OnApplicationQuit()
    {
        listener.Close();
    }

}

