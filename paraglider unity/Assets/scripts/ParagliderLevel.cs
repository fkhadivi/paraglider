using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

//[ExecuteInEditMode]
public class ParagliderLevel : MonoBehaviour {

	 
	public string level;
    public Scene scene;
	[Header("Terrain Settings")]
	public GameObject terrain;
	public GameObject startPoint;
	public ParagliderLevelPreview levelPreview;
	public ParagliderFinish finish;
	public GameObject markerNE;
	public GameObject markerSW;

	public Sprite map;
	public Vector2 scale;
	//public List<Obstacle> obstacles = new List<Obstacle>();
	[Header("Obstacles")]
	public ParagliderFlyingObjSpawnPoint monileObjectsSpawnPoints;
	public mobileObstacle[] flyingObstacles = new mobileObstacle[12] ;
	public staticObstacle[] staticObstacles = new staticObstacle[12] ;
	public List<ParagliderFlyingObstacle> availableMobilObstacles;
	public RenderTexture previewTexture;

	public delegate void Delegate();
    public Delegate onSetupFinished;
	public Delegate onLevelAwake;



	[System.Serializable]
	public class mobileObstacle
	{
		public GameObject Object;
		 
		public int maxAppearances=0;
		public float maxSpeed=0;
		public bool canAppear=false;

		public mobileObstacle ()
		{
			maxAppearances=0;
			maxSpeed=0;
			canAppear=false;
//			GameObject = null;a
		}

	}

    [System.Serializable]
    public class staticObstacle
    {
        public ParagliderStaticObstackles Object;
        public int maxAppearances = 0;

        public void distribute()
        {
            if (Object != null)
                maxAppearances = Object.distribute(maxAppearances);
        }

    }


	public void orderMobileObstacles(mobileObstacle[] Array)
	{
		availableMobilObstacles.Clear();
		for(int i = 0;i<Array.Length;i++)
		{
			if (Array[i].Object!=null && Array[i].maxAppearances>0)
			{
				for (int j=0;j<Array[i].maxAppearances;j++)
				{
					availableMobilObstacles.Add(Array[i].Object.GetComponent<ParagliderFlyingObstacle>());
                    //Debug.Log("█ available onject added:" + availableMobilObstacles[availableMobilObstacles.Count-1]);
                }
			}
		}
	}

	public ParagliderFlyingObstacle spawnFlyingObject()
	{
        if (availableMobilObstacles.Count > 0)
        {
            int i = UnityEngine.Random.Range(0, availableMobilObstacles.Count);
            Debug.Log("level "+SceneManager.GetActiveScene().name+" - fetching index " + i + " out of " + availableMobilObstacles.Count + "available objetcs");
            GameObject copy = Instantiate(availableMobilObstacles[i].gameObject);
            copy.transform.parent = terrain.transform;
            copy.SetActive(true);
            ParagliderFlyingObstacle output = copy.GetComponent<ParagliderFlyingObstacle>();
            availableMobilObstacles.RemoveAt(i);
            Debug.Log(i + "of" + availableMobilObstacles.Count + " available objects is a " + copy.name + " with script " + output + "is ready to be spawned");
            output.spawn();
            return output;
        }
        return null;
	}





	public void distributeStatObst()
	{
		foreach( staticObstacle obst in staticObstacles)
		{
			obst.distribute();
		}
	}



	public void makeLevelPreview()
	{
		//put the level preview (with cam) to the point where the paraglider will appear
		if(startPoint == null)
		Debug.LogError(level+"has no start point, put gameobject in from inspector");
		if(levelPreview == null)
		return;
		levelPreview.transform.position=startPoint.transform.position;
		levelPreview.transform.rotation=startPoint.transform.rotation;
        previewTexture = levelPreview.getPreviewTexture();

    }

	// Use this for initialization
	public ParagliderLevel Setup () {
		Debug.Log(">>>>>>>>>>> setting up Level ("+gameObject.name+") <<<<<<<<<<<<<<<");

		if (terrain != null)
        {
			terrain.SetActive(true);
        }
		waitForDelayedSetup = true;
        return this;
	}

	//wait for all scripts to run and settle down
	public int delayFrames = 4; 
	bool waitForDelayedSetup = false;

	public void delayedSetup()
	{
		waitForDelayedSetup=false;
		distributeStatObst();
		orderMobileObstacles(flyingObstacles);
		if (terrain != null)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(gameObject.scene);
			makeLevelPreview();
            //Debug.Log("preview level " + level);
            //Debug.Break();
            SceneManager.SetActiveScene(currentScene);
            terrain.SetActive(false);
            onSetupFinished();
        }
	}

	bool waitForWake = false;

	public void wake()
	{
		Debug.Log("public void wake(RenderTexture previewTexture)");
		terrain.SetActive(true);
		waitForWake = true;
		delayFrames = 2;
	}

	public void delayedAwake()
	{
		if(levelPreview!= null)
		{
			levelPreview.gameObject.SetActive(false);
		}

		onLevelAwake();
	}

	// Update is called once per frame
	void Update () 
	{
		if (waitForDelayedSetup && delayFrames-- <0)
		{
			delayedSetup();
		}
		else if (waitForWake && delayFrames-- <0) 
		{
			waitForWake = false;
			delayedAwake();
		}
	}
}
