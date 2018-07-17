using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParagliderThermic : MonoBehaviour {

	public ParagliderControler glider;
    bool hasStarted = false;
    float smoothing = 0.8f;
    public bool appeared = false;
    public float nullAlt = -100;
	private bool isConnected = false;

	// Use this for initialization
	void Start () {
        hasStarted = true;
        glider = findGlider();
        if(!appeared)
        {
			transform.position = new Vector3(transform.position.x,nullAlt,transform.position.z);
		}
		this.connect(true);
	}

	void  onDestroy()
	{     
		this.connect(false);
	}

	private void OnEnable()
    {
        if (hasStarted) return;
        Start();
    }

	public void connect(bool shouldBeConnected)
    {
		if(isConnected == shouldBeConnected) return;
		if(shouldBeConnected) 
		{
			glider.onThermicAppear += this.appear;
			glider.onThermicDisappear += this.disappear;
		}
		else 
		{
			glider.onThermicAppear -= this.appear;
			glider.onThermicDisappear -= this.disappear;

		}
		isConnected = shouldBeConnected;
    }

	ParagliderControler findGlider()
    {
		return FindObjectOfType<ParagliderControler>();
    }

    public void moveTo(float altitude)
    {
		Vector3 pos = transform.position;
		pos.y = Mathf.Lerp(altitude, pos.y, smoothing);
		transform.position = pos;

    }

    public void appear()
	{appeared=true;}

	public void disappear()
	{appeared=false;}



    // Update is called once per frame
    void Update () {
    	if(appeared)
    	{
			moveTo(nullAlt);
    	}
    	else
    	{
			moveTo(glider.transform.position.y);
		}
	}
}
