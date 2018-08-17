using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ParagliderMainScript : MonoBehaviour {


    private static ParagliderMainScript instance;

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
	public float gameTime =0;						//zeit des spiels (countdown)

	public Sprite mapTexture; //wird aus level info des levels übernommen und an map übergeben

    public bool spawnByCollider = false;	//dürfen collider spawn von Hindernissen auslösen
    public bool spawnByTime = false;		//wird automatisch nach einer gewissen Zeit ein Hinderniss gespawned
    public float maxTimeToSpawn = 6.0f;		//mindestzeit zwischen dem spawnen zweier hindernisse, vor ablauf werden collider ignoriert
	public float timeToSpawn = 6.0f;		//Zeit bis zum nächsten spawn

	public bool debug = false;				// sollen debug daten erscheinen
    public bool finishGame = false;         // spiel wird beendet, kein weitere spawning
    private bool godmode = false;

    public STATE state;

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

    public static ParagliderMainScript GetInstance()
    {
        return instance;
    }

    void Start()
    {
        instance = this;

        maxGameTime = (float)Configuration.GetInnerTextByTagName("maxGameTime", maxGameTime);

        if (!created && dontDestroyOnLoad)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
            Debug.Log("Awake: " + this.gameObject);

        }
		Map.Setup(mapTexture,new Vector3(-1000,0,-1000),new Vector3(1000,0,1000));
		gliderRig=glider.GetComponent<Rigidbody>();
		ParagliderCrashDetection CrashDetect = glider.GetComponent<ParagliderCrashDetection>();
		glider.onCrash = onCrash;
		//glider.onFog = onFog;
		glider.onFinishReached = onFinish;
        glider.onThermicDown = onThermic;
        glider.onThermicUp = onThermic;
        //glider.onThermicLeave = onThermicLeave;
        glider.onSpawn = onSpawn;
        glider.onDocking = onDocking;
        levelControl = GetComponent<ParagliderLevelControl>();
        //gameReset();
    }

    /// <summary>
    ///  can only be called when IGP is visible because it takes time and things are visible
    /// </summary>
    public void gameReset()
    {
        instance.debug =false;
        instance.onDebugChange(false);
        //onDoldrums();
        instance.glider.togglePhysics(false);
        instance.levelControl.onPreloadDone = instance.onLevelsLoaded;
        instance.levelControl.onLevelAwake = instance.onLevelawake;
        instance.gameTime = 0;
        instance.levelControl.goToLevel(0);
        instance.levelControl.preloadAllLevels();
        instance.debugInfo.log("last big event", "game reset started");
        //HUD.setGameTime(0);
        instance.HUD.appearHIDDEN();
        instance.gamePause(true);

        // state can be used by GameManager to see if main is ready
        instance.changestate(STATE.RESETTING);
    }

    /// <summary>
    /// 03.00 Spielstart
    /// Prepare before start and set to start point
    /// </summary>
    public void gameStart()
    {
        HUD.appearCOMPLETE();
        GameManager.GetInstance().ChangePromptInGame(1); //03.00 Spielstart
        NextLevel();
        gamePause(true);
        changestate(STATE.ATSTART);
    }

    /// <summary>
    /// 04.00 Welt 1: Steuerung
    /// Actually set the paraglider free
    /// </summary>
    public void gameStartPlaying()
    {
        GameManager.GetInstance().ChangePromptInGame(2);//04.00 Welt 1: Steuerung (thermics)
        gamePause(false);
        changestate(STATE.PLAYING);
    }


    public bool pausing = false;

    /// <summary>
    /// No STATE change
    /// stores current physics values and turns off/on aeroplane scripts
    /// 07.00 Spielende
    /// 08.00 crasg / Game Over
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
        HUD.appearINFOONLY();
<<<<<<< HEAD
        changestate(STATE.CRASH);     
=======
        GameManager.GameOver(false);
>>>>>>> a1dc6fda25e4951be49721734feef88a3522d86c
    }

	public void onFog()
    {
		Debug.Log("FOG");
        debugInfo.log("collider info", "fog");
    }

    /// <summary>
    /// level change or
    /// 07.00 Spielende
    /// </summary>
	public void onFinish()
    {
        debugInfo.log("collider info", "finish reached");
		Debug.Log("level "+levelControl.level+" finish reached");
        if (!finishGame )
    	{
    		NextLevel();
    	}
        else
        {
            //07.00 Spielende
            HUD.appearHIDDEN();
            changestate(STATE.FINISH);
        }
    }

    /// <summary>
    /// 09.00 Zeit abgelaufen
    /// call this when time is max game time
    /// </summary>
    public void onTimeout()
    {
        GameManager.GetInstance().ChangePromptInGame(9);//09.00 Zeit abgelaufen
        GameManager.GameOver(true);
        onDoldrums();
    }

    /// <summary>
    /// called when started to dock to the level changing frame
    /// </summary>
	void onDocking()
    {
		debugInfo.log("last big event", "started docking to level frame for level change");
    }

    /// <summary>
    /// spawn new obstacle
    /// </summary>
	public void onSpawn()
	{
        if (!finishGame && !(timeToSpawn>0))
        {
			GameObject spawned = levelControl.levelInfo().spawnFlyingObject().gameObject;
			if(debug && spawned!= null)
            debugInfo.log("spawned", spawned.name );
            
            Debug.Log("triggered spawn point");
			timeToSpawn=maxTimeToSpawn;
        }
    }


    /*
         ▀▀█▀▀  █  █  █▀▀▀  █▀▀▄  █▄ ▄█  ▀█▀  ▄▀▀▄  ▄▀▀▀ 
           █    █▀▀█  █▀▀   █▀▀▄  █▀▄▀█   █   █     ▀▀▀▄ 
           █    █  █  █▄▄▄  █  █  █ █ █  ▄█▄  ▀▄▄▀  ▀▄▄▀ 
    */

    public bool thermicBeenUsed = false;
    public void onThermic()
    {
        if (!thermicBeenUsed)
        {
            thermicBeenUsed = true;
            //blendOutHeadline
            GameManager.GetInstance().ChangePromptInGame(0);
            HUD.appearNOINFO();
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
        if (levelControl.level > 0)
        {
            Map.Setup(levelControl.levelInfo().map,
                            levelControl.levelInfo().markerNE.transform.position,
                            levelControl.levelInfo().markerSW.transform.position
                            );
            HUD.setLevelString("Welt " + levelControl.level.ToString() + "/4");
            glider.spawn(levelControl.levelInfo().startPoint.transform);
            if (levelControl.level > 1) gamePause(false);


            levelControl.wakeLevel();

            Map.gameObject.SetActive(true);

            if (levelControl.level == 1)
            {
                gameTime = maxGameTime;

            }
        }
        else
        {
            //this is standby level
            Map.gameObject.SetActive(false);
            HUD.setLevelString("");

            HUD.appearHIDDEN();
        }

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
    public delegate void boolDelegate(bool b);
    public boolDelegate onDebugChange;
   
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
    	  	debug=!debug;
    	  	onDebugChange(debug);
    	}
		if (Input.GetKeyDown("r")) 
    	{
    	  	glider.respawn();
    	}
		if (Input.GetKeyDown("s")) 
    	{
    	  	onSpawn();
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
            debugInfo.log("godmode", godmode ? "ensbled" : "disabled");
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



    void updateHUD(bool reset = false)
    {
        if (reset)
        { }
        else
        {

            BenjasMath.countdownToZero(ref gameTime);
            debugInfo.log("time", gameTime);
            //the finish looks same in all directions 
            // so i can misuse that to get the angle towards the glider 
            levelControl.levelInfo().finish.transform.LookAt(glider.transform.position);
            //and then rotate it 180 to get the direction towards the finish
            compass.setBacon(levelControl.levelInfo().finish.transform.eulerAngles.y + 180 - glider.transform.eulerAngles.y);
            compass.setCompass(glider.transform.eulerAngles.y);
            HUD.setGameTime(1 - gameTime / maxGameTime);
            Map.updateMap(glider.position, glider.transform.eulerAngles);

            debugInfo.log("compass angle", glider.transform.eulerAngles.y);
            debugInfo.log("speed", glider.speed);
            debugInfo.log("altitude", glider.altitude);
            debugInfo.log("alt change", glider.altChange);
        }

    }



    void Update()
    {

        cheatkeys();

        if (levelControl.level > 0)
        {
            //BUG BUG BUG
            //only update after levels have been loaded, never during load
            updateHUD();
        }




        if (BenjasMath.countdownToZero(ref timeToSpawn) && spawnByTime)
        {
            onSpawn();
            // switch off spawning by time after first object and switch on spawning when hitting a collider
            spawnByTime = false;
            spawnByCollider = true;
        }

        //debugInfo.log("time", levelControl.levelInfo().terrain.activeInHierarchy?"visible":"invisible");
    }


}
