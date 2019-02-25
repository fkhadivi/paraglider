using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using IGP;

public class ParagliderGame : MonoBehaviour {


    private static ParagliderGame instance;

    public bool dontDestroyOnLoad = false;
    private static bool created = false;
    ParagliderLevelControl levelControl;
	public ParagliderControler glider;			//script für paraglider interaktionen mit umgebung
	Rigidbody gliderRig;						//rigidbody of glider
	public GameMap_benja Map;					//steuert die karte
	public paragliderHUD HUD;			        //steuert den höhenmesser etc
    public compassStrip_benja compass;
    public DebugInfo_benja debugInfo;			// gibt debug daten auf dem Bildschirm aus
												// DebugInfo.Log("Zeile","WERT") Befehl erstellt beim ersten Aufruf 
												// die Zeile "Zeile" und danach überschreibt es bei jedem aufruf 
												//den wert dieser Zeile

	public float maxGameTime= 180.0f;			//maximale zeit des spiels 
	public float gameTime =0;                   //zeit des spiels (countdown)
    public float distanceFinishNear = 100;      //needed for giving a message when the finish will be near
    bool finishIsNear = false;
    public bool thermicBeenUsed = false;

    public Sprite mapTexture; //wird aus level info des levels übernommen und an map übergeben

    public bool spawnByCollider = false;	//dürfen collider spawn von Hindernissen auslösen
    public bool spawnByTime = false;		//wird automatisch nach einer gewissen Zeit ein Hinderniss gespawned
    public float maxTimeToSpawn = 6.0f;		//mindestzeit zwischen dem spawnen zweier hindernisse, vor ablauf werden collider ignoriert
	public float timeToSpawn = 6.0f;        //Zeit bis zum nächsten spawn

    public bool pausing = false;
    public bool debug = false;				// sollen debug daten erscheinen
    public bool finishGame = false;         // spiel wird beendet, kein weitere spawning
    private bool godmode = false;
    public delegate void boolDelegate(bool theBool);
    public boolDelegate onDebugChange;

    public STATE state;
    public int currentLevel = 0;

    public enum STATE
    {
        NONE,
        RESETTING,
        READY,
        ATSTART,
        PLAYING,
        CRASH,
        FINISH,
        TIMEOUT
    }

    void changestate(STATE newState)
    {
        state = newState;
        debugInfo.log("state", state.ToString());
    }

    public static ParagliderGame GetInstance()
    {
        return instance;
    }

    public void getValuesFromConfig()
    {
        maxGameTime = (float)Configuration.GetInnerTextByTagName("maxGameTime", maxGameTime);
        maxTimeToSpawn = (float)Configuration.GetInnerTextByTagName("maxTimeToSpawn", maxTimeToSpawn);
        glider.dockingDuration = (float)Configuration.GetInnerTextByTagName("dockingDuration", glider.dockingDuration);
        glider.fogDuration = (float)Configuration.GetInnerTextByTagName("fogDuration", glider.fogDuration);
        glider.maxThermicHeight = (float)Configuration.GetInnerTextByTagName("maxThermicHeight", glider.maxThermicHeight);
        glider.minThermicHeight = (float)Configuration.GetInnerTextByTagName("minThermicHeight", glider.minThermicHeight);
        glider.forceOnThermicUp = (float)Configuration.GetInnerTextByTagName("forceOnThermicUp", glider.forceOnThermicUp);
        glider.forceOnThermicDown = (float)Configuration.GetInnerTextByTagName("forceOnThermicDown", glider.forceOnThermicDown);
        glider.forceOnMaxHeight = (float)Configuration.GetInnerTextByTagName("forceOnMaxHeight", glider.forceOnMaxHeight);
        glider.collisionMaxAngleForCrash = (float)Configuration.GetInnerTextByTagName("collisionMaxAngleForCrash", glider.collisionMaxAngleForCrash);
        glider.minCrashThresholdMPS = (float)Configuration.GetInnerTextByTagName("minCrashThresholdMPS", glider.minCrashThresholdMPS);
}

    void Start()
    {
        instance = this;
        onDebugChange += instance.debugInfo.setDebugState;
        getValuesFromConfig();

        if (!created && dontDestroyOnLoad)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
            Debug.Log("Awake: " + this.gameObject);
        }
		Map.Setup(mapTexture,new Vector3(-1000,0,-1000),new Vector3(1000,0,1000));
		gliderRig=glider.GetComponent<Rigidbody>();
		//ParagliderCrashDetection CrashDetect = glider.GetComponent<ParagliderCrashDetection>();
		glider.onCrash = onCrash;
		//glider.onFog = onFog;
		glider.onFinishReached = onLevelFinish;
        glider.onThermicDown = onThermic;
        glider.onThermicUp = onThermic;
        //glider.onThermicLeave = onThermicLeave;
        glider.onSpawn = onSpawn;
        glider.onDocking = onDocking;
        levelControl = GetComponent<ParagliderLevelControl>();
        gameReset();
    }

    /// <summary>
    ///  can only be called when IGP is overlaying whole screen 
    ///  because it takes several seconds and things will be visible
    /// </summary>
    public void gameReset()
    {
        instance.debug =false; 
        instance.onDebugChange(false);
        //onDoldrums();
        instance.glider.togglePhysics(false);
        instance.thermicBeenUsed = false;
        instance.levelControl.onPreloadDone = instance.onLevelsLoaded;
        instance.levelControl.onLevelAwake = instance.onLevelawake;
        instance.gameTime = 0;
        instance.levelControl.goToLevel(0);
        currentLevel = 0;
        instance.levelControl.preloadAllLevels();
        instance.debugInfo.log("last big event", "game reset started");
        //HUD.setGameTime(0);
        instance.HUD.appearHIDDEN();
        updateHudTexts();
        instance.gamePause(true);
        instance.finishIsNear = false;
        // state can be used by GameManager to see if main is ready
        instance.changestate(STATE.RESETTING);
    }



    /// <summary>
    /// toggle debug true or false
    /// </summary>
    /// <param name="setDebug"></param>
    void setDebug(bool setDebug)
    {
        instance.debug = setDebug;
        instance.onDebugChange(setDebug);
    }

    public string stateToString(STATE state)
    {
        switch (state)
        {
            case STATE.ATSTART: return "0300";
            case STATE.CRASH: return "0800";
            case STATE.FINISH: return "0600";
            case STATE.PLAYING: return "0400";
            case STATE.TIMEOUT: return "0900";
            default: return "0000";
        }
    }

    public void updateHudTexts()
    {
        // 0404.MT.0600.L.01[Höhe] m
        //0404.MT.0600.L.02[Geschwindigkeit] km / h
        //0404.MT.0600.L.03   m / sek
        //0404.MT.0600.C  Welt[Zahl] / 4
        // TextProvider.GetText("")


        string alt      = "0404.MT." + stateToString(state) + ".L.01";
        string speed    = "0404.MT." + stateToString(state) + ".L.02";
        string level    = "0404.MT." + stateToString(state) + ".C";
        HUD.altUnit = TextProvider.GetText(alt);
        HUD.speedUnit = TextProvider.GetText(speed);
        HUD.setLevelString(TextProvider.GetText(level) + " " + currentLevel + "/" + levelControl.getLastLevel());
    }

    /// <summary>
    /// 03.00 Spielstart
    /// Prepare before start and set to start point
    /// </summary>
    public void gameStart()
    {
        changestate(STATE.ATSTART);
        NextLevel();     
        HUD.appearCOMPLETE();
        updateHudTexts();
        updateHUD();
        GameManager.ChangePromptTextInIGP(1); //03.00 Spielstart
        spawnByCollider = false;
        spawnByTime = false;
        gamePause(true);
    }

    /// <summary>
    /// 04.00 Welt 1: Steuerung
    /// Actually set the paraglider free
    /// </summary>
    public void gameStartPlaying()
    {
        GameManager.ChangePromptTextInIGP(2);//04.00 Welt 1: Steuerung (thermics)
        gamePause(false);
        changestate(STATE.PLAYING);
        // set a countdown to spawn afer certain amount of time
        spawnByCollider = false;
        spawnByTime = true;
        timeToSpawn = maxTimeToSpawn;

        // start velocity -> fix to make it easier to steer
        gliderRig.AddRelativeForce(Vector3.forward * (float)Configuration.GetInnerTextByTagName("startSpeed", 2000.0)) ;
    }

    /// <summary>
    /// resume After Pause
    /// </summary>
    public void gameResume()
    {
        gamePause(false);
        // start velocity -> fix to make it easier to steer
        gliderRig.AddRelativeForce(Vector3.forward * 0.5f* (float)Configuration.GetInnerTextByTagName("startSpeed", 2000.0));
    }

    


    /// <summary>
    /// No STATE change
    /// stores current physics values and turns off/on aeroplane scripts
    /// 07.00 Spielende
    /// 08.00 crash / Game Over
    /// 09.00 time out/game over
    /// 10.00 Abbruch
    /// 11.00 Inaktivität
    /// 
    /// </summary>
    /// <param name="doPause"></param>
    public void gamePause(bool doPause = true)
    {
        pausing = doPause;
        glider.togglePhysics(!pausing);
    }


	/*
	 ▄▀▀▄  █▀▀▄  ▄▀▀▀  ▀▀█▀▀  ▄▀▀▄  ▄▀▀▄  █     █▀▀▀  ▄▀▀▀ 
	 █  █  █▀▀▄  ▀▀▀▄    █    █▀▀█  █     █     █▀▀   ▀▀▀▄ 
	 ▀▄▄▀  █▄▄▀  ▀▄▄▀    █    █  █  ▀▄▄▀  █▄▄▄  █▄▄▄  ▀▄▄▀ 
	*/


	public void onCrash()
    {
        debugInfo.log("collider info", "crash");
        if (godmode) return;
        changestate(STATE.CRASH);
        HUD.appearINFOONLY();
        updateHudTexts();
        GameManager.GameOver(false);
    }

	public void onFog()
    {
		Debug.Log("FOG");
        debugInfo.log("collider info", "fog");
    }



    /// <summary>
    /// 06.00 Welt 4: Ziel
    /// called when finish is close
    /// </summary>
    void onFinalFinishNear()
    {
        if (!finishIsNear)
        {
            finishIsNear = true;
            HUD.appearCOMPLETE();
            updateHudTexts();
            GameManager.ChangePromptTextInIGP(4);//06.00 Welt 4: Ziel
        }
    }

    /// <summary>
    /// called when level finish is reached
    /// </summary>
    public void onLevelFinish()
    {
        debugInfo.log("collider info", "finish reached");
        Debug.Log("level " + levelControl.level + " finish reached");
        if (!finishGame)
        {
            if (levelControl.incrementLevel() != 0) NextLevel();
            else onFinalFinish();
        }
    }



    /// <summary>
    /// called on final finish
    /// 07.00 Spielende
    /// </summary>
    public void onFinalFinish()
    {
        gamePause();
        changestate(STATE.FINISH);
        HUD.appearINFOONLY();
        updateHudTexts();
        GameManager.GoToResult(gameTime);
    }


    /// <summary>
    /// 09.00 Zeit abgelaufen
    /// call this when time is max game time
    /// </summary>
    public void onTimeout()
    {
        onDoldrums();
        HUD.appearINFOONLY();
        updateHudTexts();
        GameManager.ChangePromptTextInIGP(5);//09.00 Zeit abgelaufen
        GameManager.GameOver(true);
        
    }



    /// <summary>
    /// called when started to dock to the level changing frame
    /// </summary>
    void onDocking()
    {
        debugInfo.log("last big event", "started docking to level frame for level change");
        gamePause();
    }

    /// <summary>
    /// spawn new obstacle
    /// spawnByCollider must be true
    /// </summary>
	public void onSpawn()
	{
        if (spawnByCollider)
        {
            GameObject spawned = makeSpawn();
            if (debug && spawned!= null)
            debugInfo.log("spawned", spawned.name );
            
            Debug.Log("triggered spawn point");
			timeToSpawn=maxTimeToSpawn;
        }
    }

    private GameObject makeSpawn() { return levelControl.levelInfo().spawnFlyingObject().gameObject; }

    /*
         ▀▀█▀▀  █  █  █▀▀▀  █▀▀▄  █▄ ▄█  ▀█▀  ▄▀▀▄  ▄▀▀▀ 
           █    █▀▀█  █▀▀   █▀▀▄  █▀▄▀█   █   █     ▀▀▀▄ 
           █    █  █  █▄▄▄  █  █  █ █ █  ▄█▄  ▀▄▄▀  ▀▄▄▀ 
    */

    

    public void onThermic()
    {
        if( !thermicBeenUsed && state == STATE.PLAYING)
        {
            thermicBeenUsed = true;
            //blendOutHeadline
            GameManager.ChangePromptTextInIGP(0);
            HUD.appearNOINFO();
            updateHudTexts();
        }
    }

    public bool thermics = false;

    public void onDoldrums() //FLAUTE
    {
    	thermics=false;
		Debug.Log("thermics "+thermics.ToString());
		debugInfo.log("thermics",thermics.ToString());
		glider.onThermicDisappear(); 
    }        

    public void onStiffBreeze() //STEIFE BRIESE ;)
    {
		thermics=true;
		Debug.Log("thermics "+thermics.ToString());
		glider.onThermicAppear();

    }

    /*
 █     █▀▀▀  █  █  █▀▀▀  █     ▄▀▀▀ 
 █     █▀▀   █ █   █▀▀   █     ▀▀▀▄ 
 █▄▄▄  █▄▄▄   █    █▄▄▄  █▄▄▄  ▀▄▄▀ 
*/

    void onLevelsLoaded()
    {
        changestate(STATE.READY);
        debugInfo.log("last big event", "all levels preloaded");
    }

    /// <summary>
    /// will start the next level in order of 1,2...last,0,1,2...
    /// </summary>
    public void NextLevel()
    {
        Debug.Log("████████████  LEVEL UP  ████████████");
        debugInfo.log("last big event", "level started");
        levelControl.NextLevel();
        currentLevel = levelControl.level;
        if (currentLevel > 0)
        {
            Map.Setup(levelControl.levelInfo().map,
                            levelControl.levelInfo().markerNE.transform.position,
                            levelControl.levelInfo().markerSW.transform.position
                            );
            updateHudTexts();
            glider.spawn(levelControl.levelInfo().startPoint.transform);
            if (currentLevel > 1) gamePause(false);

            levelControl.wakeLevel();

            Map.gameObject.SetActive(true);

            if (currentLevel == 1)
            {
                gameTime = 0;
            }
        }
        else
        {
            //this is standby level
            Map.gameObject.SetActive(false);
            HUD.appearHIDDEN();
            updateHudTexts();

        }


        // start velocity -> fix to make it easier to steer
        glider.setSpeed(0);
        gliderRig.AddRelativeForce(Vector3.forward * (float)Configuration.GetInnerTextByTagName("startSpeed", 2000.0));

        Debug.Log("level " + levelControl.level + " started");
        debugInfo.log("level", "" + levelControl.level);
        onDoldrums();
        
    }

    void onLevelawake()
    {
        debugInfo.log("last big event", "level running");
        glider.togglePhysics(!pausing);
        //this will show the winds
        onStiffBreeze();
    }

    /*
         █  █  ▄▀▀▄  ▀▀█▀▀  █ ▄▀  █▀▀▀  █  █  ▄▀▀▀ 
         █▀▀█  █  █    █    █▀▄   █▀▀   █▄▀   ▀▀▀▄ 
         █  █  ▀▄▄▀    █    █ ▀▄  █▄▄▄  █     ▀▄▄▀ 
    */


    private bool hotkeysToDebug = true;

   
    public void cheatkeys()
    {
        if (hotkeysToDebug)
        {
            hotkeysToDebug = false;
            debugInfo.log(": CHEATKEYS","");
            debugInfo.log("key D", "toggle Debug");
            debugInfo.log("key R", "respawn glider");
            debugInfo.log("key S", "spawn obstacle");
            debugInfo.log("key L", "next level");
            debugInfo.log("key X", "reset game");
            debugInfo.log("key g", "toggle godmode");
			debugInfo.log("key p", "toggle physics");
			debugInfo.log("key q/w", "quiet<>windy ");
			debugInfo.log("key space", "superspeed");
            debugInfo.log("key 1", "start game");
            debugInfo.log("key 2", "start playing game (start glider)");
            debugInfo.log(": :",": : : : :");
        }
        if (Input.GetKeyDown("1"))
        {
            gameStart();
        }

        if (Input.GetKeyDown("2"))
        {
            gameStartPlaying();
        }

        if (Input.GetKeyDown("d")) 
    	{
    	  	setDebug(!debug);
    	}
		if (Input.GetKeyDown("r")) 
    	{
    	  	glider.respawn();
    	}
		if (Input.GetKeyDown("s")) 
    	{
            makeSpawn();
    	}
		if (Input.GetKeyDown("l")) 
    	{
			NextLevel();
    	}
        if (Input.GetKeyDown("x"))
        {
            gameReset();
        }
        if (Input.GetKeyDown("g"))
        {
            godmode = !godmode;
            glider.crashEnabled = !godmode;
            debugInfo.log("godmode", godmode ? "enabled" : "disabled");
        }
		if (Input.GetKeyDown("w"))
        {
        	onStiffBreeze();
        }
		if (Input.GetKeyDown("q"))
        {
			onDoldrums();
        }
        if (Input.GetKeyDown("p"))
        {
            gamePause(!pausing);
				debugInfo.log("physics", glider.physicsEnabled()? "enabled" : "disabled");
        }
		if (!glider.physicsEnabled())
		{
		if (Input.GetKey("space")) glider.transform.position += glider.transform.forward*10;
		if (Input.GetKey("up")) glider.transform.eulerAngles += new Vector3(+3,0,0);
			if (Input.GetKey("down")) glider.transform.eulerAngles += new Vector3(-3,0,0);
			if (Input.GetKey("left")) glider.transform.eulerAngles += new Vector3(0,-3,0);
			if (Input.GetKey("right")) glider.transform.eulerAngles += new Vector3(0,+3,0);
		}

	}


    /// <summary>
    /// BUG BUG BUG
    /// only update after levels have been loaded, never during load 
    /// </summary>
    /// <param name="reset"></param>
    /// <param name="timeNormalized"></param>
    void updateHUD( float timeNormalized=0)
    {
         if(state != STATE.RESETTING) //never update during loading levels, buggy
         {
            HUD.setGameTime(timeNormalized);
            debugInfo.log("time", gameTime);
            //make we put a bacon game object to the finish 
            GameObject bacon1 = new GameObject();
            bacon1.transform.position = levelControl.levelInfo().finish.transform.position;
            // and let it look at the glider
            bacon1.transform.LookAt(glider.transform.position);
            //and then rotate its y 180 to get the direction from glider towards the finish
            // and now the difference of the two y angles is the angle of the bacon
            compass.setBacon(bacon1.transform.eulerAngles.y + 180 - glider.transform.eulerAngles.y);
            Destroy(bacon1);
            compass.setCompass(glider.transform.eulerAngles.y);
            
            Map.updateMap(glider.position, glider.transform.eulerAngles);

            debugInfo.log("compass angle", glider.transform.eulerAngles.y);
            debugInfo.log("speed", glider.speed);
            debugInfo.log("altitude", glider.altitude);
            debugInfo.log("alt change", glider.altChange);
        }

    }

    public bool stopEveryFrame = false;

    void Update()
    {
        if (stopEveryFrame) Debug.Break();
        cheatkeys();

        if (state == STATE.PLAYING)
        {
            float normalizedGametime = BenjasMath.timer(ref gameTime, maxGameTime, pausing);
            updateHUD( normalizedGametime);
            if(normalizedGametime == 1)
            {
                onTimeout();
            }

            if (!pausing)
            {
                if (spawnByTime && BenjasMath.countdownToZero(ref timeToSpawn))
                {
                    // switch off spawning by time after first object and switch on spawning when hitting a collider
                    spawnByTime = false;
                    spawnByCollider = true;
                    //spawn first object
                    onSpawn();
                }
                if(levelControl.isLastLevel())
                {
                    if (Vector3.Distance(glider.transform.position, levelControl.levelInfo().finish.transform.position) < distanceFinishNear)
                    {
                        onFinalFinishNear();
                    }
                }
            }

        }
        else if (state == STATE.ATSTART)
        {
            updateHUD();
        }


    }

}
