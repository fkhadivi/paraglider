using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ParagliderLevelControl : MonoBehaviour {

    private static bool created = false;

	public string[] levelNames;
    private Scene[] levels = new Scene[1];
    public ParagliderLevel[] levelInfos;
	public int level = -1;

    public delegate void Delegate();
    public Delegate onPreloadDone;

    private void Start()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
        Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
    }

  

    public ParagliderLevel levelInfo()
    {
        if (level > 0)
        return levelInfos[level];
        return null;
    }

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

    public void preloadAllLevels()
    {
        Debug.Log("preloading levels");
        if (!(levels.Length == levelNames.Length))
        {
            levels = new Scene[levelNames.Length];
            levels[0] = SceneManager.GetSceneByName(levelNames[0]);
            levelInfos = new ParagliderLevel[levelNames.Length];
        }
          isPreloadingLevels = true;
        preloadingLevel = 1;
        managePreloading();
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
        Debug.Log("level " + scene.name + " loaded");
        levels[preloadingLevel] = scene;
        levelInfos[preloadingLevel] = getLevelInfo(levels[preloadingLevel]);
        if (levelInfos[preloadingLevel].terrain != null)
        {
            levelInfos[preloadingLevel].terrain.SetActive(false);
        }
        managePreloading();
    }

    public bool preload(int l)
    {
        preloadingLevel = l;
        if (SceneManager.GetSceneByName(levelNames[l]).isLoaded)
        {
            return false;
        }
        SceneManager.LoadSceneAsync(levelNames[l], LoadSceneMode.Additive);
        Debug.Log("loading level " + l + " (" + levelNames[l] + ")");
        return true;
    }

    public void unloadlevel(int level)
    {
        SceneManager.LoadSceneAsync(levelNames[level], LoadSceneMode.Additive);
    }


    public void NextLevel()
    {
    	if(level<0)
    	{
            // is this the first time running?
        }
        goToLevel(incrementLevel(level));
    }

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

    public int incrementLevel(int currentLevel)
    {
    	currentLevel++;
    	if (currentLevel<levelNames.Length)
    	return currentLevel;
    	return 0;
        // so ein stmhallo welt  most
        // so ein stmsdfhallo welt  must
        // so ein stmhalfdlo welt  mist
    }

}
