using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ParagliderMainScript : MonoBehaviour {


    private static bool created = false;
    ParagliderLevelControl levelControl;
	public ParagliderControler glider;			//script für paraglider interaktionen mit umgebung
	public GameMap_benja Map;					//steuert die karte
	public ALtimeter_benja Altimeter;			//steuert den höhenmesser
    public DebugInfo_benja debugInfo;			// gibt debug daten auf dem Bildschirm aus
												// DebugInfo.Log("Zeile","WERT") Befehl erstellt beim ersten Aufruf 
												// die Zeile "Zeile" und danach überschreibt es bei jedem aufruf 
												//den wert dieser Zeile

	public float maxGameTime= 180.0f;			//maximale zeit des spiels 
	public float gameTime;						//zeit des spiels (countdown)

    public float maxInactivityTime = 30f;		//max zeit ohne input bevor standby eingeleitet
	public float currentInactivityTime = 30f;	//zeit ohne bis standby eingeleitet wenn kein inpout

    public float gliderSpeed;		//geschwindigkeit des gliders in m/s
	public Vector3 gliderPosition;	//
	public float gliderAlt;			//aktuellle höhe
	public float gliderAltChange;	//änderung der höhe

	public Texture2D mapTexture; //wird aus level info des levels übernommen und an map übergeben

    public bool spawnByCollider = false;	//dürfen collider spawn von Hindernissen auslösen
    public bool spawnByTime = false;		//wird automatisch nach einer gewissen Zeit ein Hinderniss gespawned
    public float maxTimeToSpawn = 6.0f;		//mindestzeit zwischen dem spawnen zweier hindernisse, vor ablauf werden collider ignoriert
	public float timeToSpawn = 6.0f;		//Zeit bis zum nächsten spawn

    public bool respawnEnabled = false;		//auto respawn nach crash

	public bool debug = false;				// sollen debug daten erscheinen
	public bool finishing = false;			// übergang ins nächste level ist eingeleitet	
    public bool finishGame = false;			// spiel wird beendet, kein weitere spawning
    
	/*

	METHODEN

	ALLGEMEIN

    void reset() setzt das spiel auf null und läd verbrauchte levels nach
    void onInput() setzt den iput imer zurück

	DebugInfo.Log("Zeile","WERT") Befehl erstellt beim ersten Aufruf 
	die Zeile "Zeile" und danach überschreibt es bei jedem aufruf 
	den wert dieser Zeile

    LEVELS

	void onLevelsLoaded()	//wird ausgelöst wenn alle levels geladen sind
	public void NextLevel()	//startet das nächste Level

	OBSTACKLES

	public void onCrash()	//wird ausgelöst bei crash mit hindernis oder Umgebung
	public void onFinish()	//wird ausgelöst bei Treffen auf Ziel
	public void onSpawn()	//wird ausgelöst bei spawnen eines Hindernisses (kein crash s.o.)

	THERMICS    
    
    public void onThermicUp()	//wird ausgelöst bei fliegen in Aufwindzone
    public void onThermicDown() //wird ausgelöst bei fliegen in abwind zone
    public void onThermicLeave() //wird ausgelöst bei verlasen der Windzone
    public void onDoldrums() //lösst flaute aus (winde verschwinden) 
    public void onStiffBreeze() //stellt winde wieder her

    Hotkeys siehe ganz unten oder debug info

*/


    void Start()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
            Debug.Log("Awake: " + this.gameObject);

        }
		Map.Setup(mapTexture,new Vector3(-1000,0,-1000),new Vector3(1000,0,1000));
		gliderRig=glider.GetComponent<Rigidbody>();
		ParagliderCrashDetection CrashDetect = glider.GetComponent<ParagliderCrashDetection>();
		glider.onCrash = onCrash;
		glider.onFog = onFog;
		glider.onFinish = onFinish;
        glider.onThermicDown = onThermicDown;
        glider.onThermicUp = onThermicUp;
        glider.onThermicLeave = onThermicLeave;
        glider.onSpawn = onSpawn;
        levelControl = GetComponent<ParagliderLevelControl>();
        //reset();
    }



    void reset()
    {
        levelControl.onPreloadDone = onLevelsLoaded;
    	gameTime = maxGameTime;
        levelControl.goToLevel(0);
        levelControl.preloadAllLevels();
        debugInfo.log("last big event", "game reset started");
    }



    void onInput()
    {
        currentInactivityTime = maxInactivityTime;
    }

    

    Rigidbody gliderRig;

    void Update()
    {
		
		cheatkeys();
        BenjasMath.countdownToZero(ref gameTime);
        debugInfo.log("time", gameTime);

        BenjasMath.countdownToZero(ref currentInactivityTime);
        debugInfo.log("time until standby", currentInactivityTime);

        gliderPosition =glider.transform.position;
    	gliderSpeed = gliderRig.velocity.magnitude;
        gliderAlt = glider.transform.position.y;
    	gliderAltChange = gliderRig.velocity.y;
        debugInfo.log("speed", gliderSpeed);
        debugInfo.log("altitude", gliderAlt);
        debugInfo.log("alt change", gliderAltChange);

        Map.updateMap(gliderPosition,glider.transform.eulerAngles);

        if (BenjasMath.countdownToZero(ref timeToSpawn) && spawnByTime)
        {
            onSpawn();
            // switch off spawning by time after first object and switch on spawning when hitting a collider
            spawnByTime = false;
            spawnByCollider = true;
        }
        //debugInfo.log("time", levelControl.levelInfo().terrain.activeInHierarchy?"visible":"invisible");
    }

	/*
	 █     █▀▀▀  █  █  █▀▀▀  █     ▄▀▀▀ 
	 █     █▀▀   █ █   █▀▀   █     ▀▀▀▄ 
	 █▄▄▄  █▄▄▄   █    █▄▄▄  █▄▄▄  ▀▄▄▀ 
	*/

	void onLevelsLoaded()
    {
        debugInfo.log("last big event", "all levels preloaded");
    }

	public void NextLevel()
	{
        Debug.Log("████████████  LEVEL UP  ████████████");
        debugInfo.log("last big event", "level started");
        levelControl.NextLevel();
		if (levelControl.level>0)
		{
			glider.spawn(levelControl.levelInfo().startPoint.transform);
			Map.Setup(		levelControl.levelInfo().map,
							levelControl.levelInfo().markerNE.transform.position,
							levelControl.levelInfo().markerSW.transform.position
							);
			levelControl.levelInfo().terrain.SetActive(true);
	    	Map.gameObject.SetActive(true);
	    	Altimeter.gameObject.SetActive(true);
    	}
    	else
    	{
    		//this is standby level
			Map.gameObject.SetActive(false);
			Altimeter.gameObject.SetActive(false);
    	}

		Debug.Log("level "+levelControl.level+" started");
        debugInfo.log("level", "" + levelControl.level);
	}



	/*
	 ▄▀▀▄  █▀▀▄  ▄▀▀▀  ▀▀█▀▀  ▄▀▀▄  ▄▀▀▄  █     █▀▀▀  ▄▀▀▀ 
	 █  █  █▀▀▄  ▀▀▀▄    █    █▀▀█  █     █     █▀▀   ▀▀▀▄ 
	 ▀▄▄▀  █▄▄▀  ▀▄▄▀    █    █  █  ▀▄▄▀  █▄▄▄  █▄▄▄  ▀▄▄▀ 
	*/


	public void onCrash()
    {
        if (respawnEnabled || godmode)
        {
            glider.respawn();
        }
        debugInfo.log("collider info", "crash");
    }

	public void onFog()
    {
		Debug.Log("FOG");
        debugInfo.log("collider info", "fog");
    }

	public void onFinish()
    {
        debugInfo.log("collider info", "finish");
        if (!finishGame && !finishing)
    	{
    		NextLevel();
    		finishing=true;
    	}
    }

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
     
    public void onThermicUp()
    {
        debugInfo.log("thermic", "up entered");
        if (!finishGame)
        {
            glider.setThermic(1);
        }
    }

    public void onThermicDown()
    {
        if (!finishGame)
        {
            debugInfo.log("thermic", "down entered");
            glider.setThermic(-1);
        }
    }

    public void onThermicLeave()
    {
        debugInfo.log("thermic", "left thermic");
        if (!finishGame)
        {
            glider.setThermic(0);
        }
    }

    public bool thermics = false;

    public void onDoldrums() //FLAUTE
    {
    	thermics=false;
		debugInfo.log("thermics",thermics.ToString());
		glider.onThermicDisappear(); 
    }                                

    public void onStiffBreeze() //STEIFE BRIESE ;)
    {
		thermics=true;
		debugInfo.log("thermics",thermics.ToString());
		glider.onThermicAppear();

    }

/*
	 █  █  ▄▀▀▄  ▀▀█▀▀  █ ▄▀  █▀▀▀  █  █  ▄▀▀▀ 
	 █▀▀█  █  █    █    █▀▄   █▀▀   █▄▀   ▀▀▀▄ 
	 █  █  ▀▄▄▀    █    █ ▀▄  █▄▄▄  █     ▀▄▄▀ 
*/


    private bool hotkeysToDebug = true;
    private bool godmode = false;
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
			debugInfo.log("key o/p", "doldrums<>stiff breeze  ");
			debugInfo.log(": :",": : : : :");
        }
        if(Input.anyKey)
        {
            onInput();
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
            reset();
        }
        if (Input.GetKeyDown("g"))
        {
            godmode = !godmode;
            debugInfo.log("godmode", godmode ? "ensbled" : "disabled");
        }
		if (Input.GetKeyDown("p"))
        {
        	onStiffBreeze();
        }
		if (Input.GetKeyDown("o"))
        {
			onDoldrums();
        }
    }

    public delegate void boolDelegate(bool b);
    public boolDelegate onDebugChange;

}
