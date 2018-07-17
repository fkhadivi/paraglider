using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugOnly_benja : MonoBehaviour {

	public ParagliderMainScript mainScript;
	public bool isDebug = true;
    private bool isConnected = false;
	// Use this for initialization
	void Awake () {
		mainScript = FindObjectOfType<ParagliderMainScript>();
		//Debug.Log("main script " + mainScript.name);
        onDebugChange(mainScript.debug);
        connect(true);
    }

    public void connect(bool shouldBeConnected)
    {
		if(isConnected == shouldBeConnected) return;
		if(shouldBeConnected) mainScript.onDebugChange += this.onDebugChange;
		else mainScript.onDebugChange -= this.onDebugChange;
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
        isDebug = debug;
		for (int ID = 0; ID < this.transform.childCount; ID++)
	     {
        	this.transform.GetChild(ID).gameObject.SetActive(debug);
    	 }
    	 this.gameObject.SetActive(debug);
    }

}
