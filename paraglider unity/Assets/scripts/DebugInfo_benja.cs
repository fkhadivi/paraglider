using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugInfo_benja : MonoBehaviour {

    public UnityEngine.UI.Text textField;
	// Use this for initialization
	void Start () {
	    	
	}

    public bool debugging = true;

    public List<string> names;
    public List<string> values;
    public bool doUpdate = true;

    public void log(string name, float value)
    {
        log(name, (Mathf.Round(value*100)/100).ToString());
    }

    public void log(string name, string value)
    {
        if (debugging)
        {
            int index = names.IndexOf(name);
            if (index < 0)
                index = addLine(name);
            values[index] = value;
            doUpdate = true;
        }
        
    }

    int addLine(string name)
    {
        names.Add(name);
        values.Add("");
        return names.Count - 1;
    }
    private void updateText()
    {
        textField.text = "Debug Info _______________________";
        for (int i = 0; i < names.Count; i++)
        {
            textField.text += "\n" + names[i] + " : " + values[i];
        }
      
    }
    // Update is called once per frame
    void Update () {
        
        if(doUpdate)
        {
            doUpdate = false;
            updateText();
        }
        textField.enabled = debugging;
    }

    
    private bool isConnected = false;
    // Use this for initialization
    void Awake()
    {

        onDebugChange(ParagliderMainScript.GetInstance().debug);
        connect(true);
    }

    public void connect(bool shouldBeConnected)
    {
        if (isConnected)
        {
            if (!shouldBeConnected)
            {
                ParagliderMainScript.GetInstance().onDebugChange -= this.onDebugChange;
            }
        }
        else
        {
            if (shouldBeConnected)
            {
                ParagliderMainScript.GetInstance().onDebugChange += this.onDebugChange;
            }
        }
    }

    private void OnEnable()
    {
        connect(true);
    }

    private void OnDisable()
    {
        connect(false);
    }

    private void OnDestroy()
    {
        connect(false);
    }


    void onDebugChange(bool debug)
    {
        debugging = debug;
        gameObject.SetActive(debugging);

    }

}
