using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugOnly_benja : MonoBehaviour {

	public DebugInfo_benja debugManager;
	public bool isDebug = true;
    private bool isConnected = false;
    public bool disableRendererOnly = true;
    public bool includeThisObjetc = false;
	// Use this for initialization
	void Start () {

        //if there are multiple DebugInfo_benja in the game 
        //it doesnt matter aslong as 
        //they all react to the same debug toggle
        //this script will yust listen to the one debugManager it finds first
        debugManager = FindObjectOfType<DebugInfo_benja>();
		Debug.Log("debug info on " + debugManager.name);

		onDebugChange(false);
        connect(true);
    }

    public void connect(bool shouldBeConnected)
    {
		if(isConnected == shouldBeConnected) return;
        if (shouldBeConnected) {
            if (debugManager == null) Start();
            debugManager.onDebugChange += this.onDebugChange;
        }
        else debugManager.onDebugChange -= this.onDebugChange;
		isConnected = shouldBeConnected;
    }

    private void OnEnable()
    {
        connect(true);
    }
 
    private void OnDestroy()
    {
        connect(false);
    }


    void onDebugChange(bool debug)
    {
        try
        {
            if (gameObject == null)
            {
                connect(false);
                return;
            }
            isDebug = debug;
            if (disableRendererOnly)
            {
                Renderer[] renderers = GetComponentsInChildren<Renderer>();
                foreach (Renderer rendi in renderers)
                {
                    rendi.enabled = debug;
                }
                if (includeThisObjetc)
                {
                    GetComponent<Renderer>().enabled = debug;
                }
            }
            else
            {
                for (int ID = 0; ID < this.transform.childCount; ID++)
                {
                    this.transform.GetChild(ID).gameObject.SetActive(debug);
                }
                if (includeThisObjetc)
                {
                    this.gameObject.SetActive(debug);
                }
            }
        }
        catch 
        {
            connect(false);
        }

    }

}
