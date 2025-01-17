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
    public TriggerChecker trigger;



	// Use this for initialization
	void Start () {
		if (hasStarted) return;
		setUp();
	}

	void Awake ()
	{
		if (hasStarted) return;
		//setUp();
	}

	private void OnEnable()
    {
        if (hasStarted) return;
		setUp();
    }

	void  onDestroy()
	{
        this.connect(false);
	}

	public void setUp()
	{
		hasStarted = true;
        trigger = GetComponentInChildren<TriggerChecker>();
        trigger.onEnter += triggerEnter;
        //trigger.onStay += triggerStay;
        trigger.onExit += triggerExit;
        glider = findGlider();
        if(!appeared)
        {
			transform.position = new Vector3(transform.position.x,nullAlt,transform.position.z);
		}
		this.connect(true);
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
        if (!appeared || !terrainHit || pos.y > transform.position.y)
        {
            transform.position = pos;
        }
    }

    public void appear()
	{appeared=true;}

	public void disappear()
	{appeared=false;}

    public bool terrainHit = false;
    private void triggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Terrain>() != null)
        {
            terrainHit = true;
            //Debug.Log(transform.parent.name + " terrain HIT " + other.gameObject.name);
        }
        //Debug.Log(gameObject.name + " entered trigger " + other.name);
    }

    private void triggerExit(Collider other)
    {
        if (terrainHit && other.gameObject.GetComponent<Terrain>() != null)
        {
            terrainHit = false;
            //Debug.Log(transform.parent.name + " terrain unHit " +other.gameObject.name);
        }
        //Debug.Log(gameObject.name +" exited trigger "+other.name);
    }


    // Update is called once per frame
    void Update () {
    	if(appeared)
    	{
			moveTo(glider.transform.position.y);
    	}
    	else
    	{
			moveTo(nullAlt);			
		}
	}
}
