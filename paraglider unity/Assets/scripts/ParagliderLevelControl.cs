using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ParagliderLevelControl : MonoBehaviour {

    private static bool created = false;

    public string[] levelNames;
    public string[] levelNamesToLoad = new string[1];
    private Scene[] levels = new Scene[1];
    public ParagliderLevel[] levelInfos;
	public int level = -1;

    public delegate void Delegate();
    public Delegate onPreloadDone;
	public Delegate onLevelAwake;

    private void Start()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
        //Debug.Log("starting LEVEL CONTROL");
    }

    /// <summary>
    /// get
    /// </summary>
    /// <returns>level info^of certain level</returns>

    public ParagliderLevel levelInfo(int level)
    {
        if (level > 0)
        return levelInfos[level];
        return null;
    }

    /// <summary>
    /// get
    /// </summary>
    /// <returns>current level info</returns>
    public ParagliderLevel levelInfo()
    {
			return levelInfo(level);
    }

    /// <summary>
    /// returns level info of the scene
    /// </summary>
    /// <param name="scene"></param>
    /// <returns></returns>
    public ParagliderLevel getLevelInfo(Scene scene)
    { 
     
    	GameObject[] roots= scene.GetRootGameObjects();
    	foreach (GameObject root in roots)
    	{
    		ParagliderLevel[] info = root.GetComponentsInChildren<ParagliderLevel>();
    		if (info.Length > 0)
    		{
    			Debug.Log(" found Level info in "+info[0].gameObject.name+" in Level "+scene.name);
    			return info[0];
    		}
    	}
		Debug.LogError(" found no Level info in Level "+scene.name);
    	return null;
    }

    public int preloadingLevel = 0;
    public bool isPreloadingLevels = false;

    /// <summary>
    /// will preload all levels that are not loaded already
    /// will load asynchronously over time
    /// </summary>
    public void preloadAllLevels()
    {
        Debug.Log(">>>>>>>>>>>>>> PRELOADING LEVELS");
        if(LevelsRestrictedTo > 0)
        {
            //restrict levels to a single level for loading and managing
            levelNamesToLoad = new string[2];
            levelNamesToLoad[0] = levelNames[0];
            levelNamesToLoad[1] = levelNames[LevelsRestrictedTo];
        }
        else if(levelNamesToLoad.Length != levelNames.Length)
        {
            //load and manage all levels
            levelNamesToLoad = new string[levelNames.Length];
            levelNamesToLoad = levelNames;
        }

        if (!(levels.Length == levelNames.Length)) 
        {
            //compares to levelNames to see if something wrong with levels or levels restricted to single
            levels = new Scene[levelNamesToLoad.Length];
            levels[0] = SceneManager.GetSceneByName(levelNamesToLoad[0]);
            levelInfos = new ParagliderLevel[levelNamesToLoad.Length];
        }
          isPreloadingLevels = true;
        preloadingLevel = 1;
        managePreloading();
    }

    public int LevelsRestrictedTo = 0; 

    public void restrictLevelsTo(int level)
    {
        if (level == Mathf.Clamp(level, 1, levelNamesToLoad.Length - 1))
        {
            LevelsRestrictedTo = level;
        }
        else
        {
            LevelsRestrictedTo = 0;
        }
    }

    public void managePreloading()
    {
        Debug.Log("manage preloading, sart at level " + preloadingLevel );
        if (isPreloadingLevels)
        {
            if (preloadingLevel > 0)
            {
                for (int i = preloadingLevel; i < levels.Length; i++)
                {
                    if (preload(preloadingLevel) == true)
                    {
                        //now wait until level is loaded and this function will run again
                        return;
                    }
                    else
                    {
                        //level loaded already, ignore
                        Debug.Log("level " + preloadingLevel + "is already loaded, checking next");
                        preloadingLevel = incrementLevel(preloadingLevel);
                    }
                }

            }
            if (preloadingLevel == 0)
            {
                isPreloadingLevels = false;
                onPreloadDone();
                Debug.Log("finished preloading");
            }

        }
    }


    void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(">>> level " + scene.name + " loaded");
        levels[preloadingLevel] = scene;
        levelInfos[preloadingLevel] = getLevelInfo(levels[preloadingLevel]).Setup();
        //the level has been loaded and the levelInfo will do setup now, 
		//this will take some frames though, so we need to wait till setup has finished
		levelInfos[preloadingLevel].onSetupFinished = onLevelSetupFinished;
    }

    void onLevelSetupFinished()
    {
		Debug.Log(">>> level " + levels[preloadingLevel].name + " setup finished");
		//level is good to go, lets go for the next level
		managePreloading();
    }

    public bool preload(int l)
    {
        preloadingLevel = l;
        if (SceneManager.GetSceneByName(levelNamesToLoad[l]).isLoaded)
        {
            return false;
        }
        SceneManager.LoadSceneAsync(levelNamesToLoad[l], LoadSceneMode.Additive);
        Debug.Log(">>> loading level " + l + " (" + levelNamesToLoad[l] + ")");
        return true;
    }

    public void unloadlevel(int level)
    {
        SceneManager.LoadSceneAsync(levelNamesToLoad[level], LoadSceneMode.Additive);
    }

    /// <summary>
    /// will start next level
    /// </summary>
    public void NextLevel()
    {
    	if(level<0)
    	{
            // is this the first time running?
        }
        goToLevel(incrementLevel(level));
    }


    /// <summary>
    /// will start level with by int newLevel
    /// </summary>
    /// <param name="newLevel"></param>
    public void goToLevel(int newLevel)
    {
        //unload last level
        if (level > 0 && levels[level].isLoaded)
        {
            SceneManager.UnloadSceneAsync(levels[level]);
        }
        //swap levels
        level = newLevel;
        if (level > 0 && level<levels.Length && levels[level].isLoaded)
        {
            SceneManager.SetActiveScene(levels[level]);
        }
        //start current level
    }

    public bool isLastLevel()
    {
        return level == getLastLevel();
    }

    public int getLastLevel()
    {
        return levelInfos.Length - 1;
    }

    public void wakeLevel()
    {
		if(levelInfo()==null)
			return;
		
		if(level>0)
		{	
			int nextLevel =incrementLevel(level);
			if(nextLevel>0)
				levelInfo().finish.wake(levelInfo(nextLevel).previewTexture);
			else
				levelInfo().finish.wake(null);

			levelInfo().wake();
			levelInfo().onLevelAwake = this.levelAwake;
		}
		else
		{
			levelAwake();
		}
    }

    public void levelAwake()
    {
		onLevelAwake();
	}

    /// <summary>
    /// tells which level will be next
    /// </summary>
    /// <returns>next level</returns>
    public int incrementLevel()
    {
        return incrementLevel(level);
    }

    /// <summary>
    /// will tell which level will be after int level
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public int incrementLevel(int level)
    {
        level++;
    	if (level < levelNamesToLoad.Length)
    	return level;
    	return 0;
    }

}
