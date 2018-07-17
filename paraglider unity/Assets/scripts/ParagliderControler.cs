using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParagliderControler : MonoBehaviour {

    public AeroplaneController aeroplaneController;
    public AeroplaneUserControl userControl;
    public Rigidbody rigi;

    public string finishColliderName = "finishCollider";
    public string crashColliderName = "crashCollider";
    public string fogColliderName = "fogCollider";
    public string spawnCollider = "spawnCollider";
    public string thermicUpName = "thermicUpCollider";
    public string thermicDownName = "thermicDownCollider";

    public delegate void Delegate();
    public Delegate onCrash;
    public Delegate onFog;
    public Delegate onFinish;
    public Delegate onSpawn;
    public Delegate onThermicUp;
    public Delegate onThermicDown;
    public Delegate onThermicLeave;
    public Delegate onThermicDisappear;
	public Delegate onThermicAppear;

    public float respawnTimePanalalty = 5f;
    public float respawnTimer = 0;
    public float respawnHeight = 150f;
    public float maxHeight = 175f;
    public List<Vector3> respawnCollector = new List<Vector3>();
    public float forceOnThermicUp = 1000;
    public float forceOnThermicDown = -500f;
    public Vector3 forceByThermic = new Vector3();
    public float forceOnHeight = -500f;
    public bool disableCrashedObjectOnCrash = false;
    public bool disableSpawnColliderOnTrigger = true;
    public bool disableWindColliderOnTrigger = false;


    // Use this for initialization
    void Start() {
        aeroplaneController = gameObject.GetComponent<AeroplaneController>();
        userControl = gameObject.GetComponent<AeroplaneUserControl>();
        rigi = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        onCrash();
        if(disableCrashedObjectOnCrash)
        collision.collider.enabled = false;
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("-----Triggered " + col.gameObject.name   );
        if (col.gameObject.name == finishColliderName)
        {
            onFinish();
        }
        else if (col.gameObject.name == fogColliderName)
        {
            onFog();
        }
        else if (col.gameObject.name == spawnCollider)
        {
            onSpawn();
            col.enabled = !disableSpawnColliderOnTrigger;
        }
        else if (col.gameObject.name == thermicUpName)
        {
            onThermicUp();
            if(disableWindColliderOnTrigger)
                col.transform.parent.gameObject.SetActive(false);
        }
        else if (col.gameObject.name == thermicDownName)
        {
            onThermicDown();
            if(disableWindColliderOnTrigger)
                col.transform.parent.gameObject.SetActive(false);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.name == thermicUpName) onThermicLeave();
        else if (col.gameObject.name == thermicDownName) onThermicLeave();
    }

    public void setThermic(int direction) //-1 down, 0 normal, +1 up
    {
        if (direction > 0)
            forceByThermic = new Vector3(0, forceOnThermicUp, 0);
        else if (direction < 0)
        { forceByThermic = new Vector3(0, forceOnThermicDown, 0); }
        else
        { forceByThermic = new Vector3(0, 0, 0); }
    }

	public void resetRespawn()
    {
		respawnCollector.Clear();
		respawnCollector.Add (respawnInfo(transform));
    }

    public void updateRespawn()
    {
    	//save glider position every second
    	respawnTimer-=Time.deltaTime;
    	if (respawnTimer<0)
    	{ 
    		respawnTimer = 1f;
    		respawnCollector.Add (respawnInfo(transform));
    	}

		//keep glider positions for a time of maxTimePanalalty only
		if(respawnCollector.Count>respawnTimePanalalty)
		{
			respawnCollector.RemoveAt(0);
		}
    }

    public void respawn()
    {
	
    	//set glider To last respawn position in horizontal direction and flight height
		Vector3 position = respawnCollector[0];
		Vector3 rotation = respawnCollector[0];
    	rotation.x=0;
		rotation.z=0;
		position.y = respawnHeight;
		spawnGlider(position,rotation);
		resetRespawn(); //prevents from  being set into a crash situation

    }

	public Vector3 respawnInfo(Transform trafo)
    {
		Vector3 respawnInfo = transform.position;
    	respawnInfo.y = transform.eulerAngles.y; //save horizontal rotation instad of height;
    	return respawnInfo;
    }

	public void spawn(Transform trafo)
	{
        if (trafo == null)
        {
            Debug.LogError("no transform delivered, spawning at zero");
            spawnGlider(Vector3.zero, Vector3.zero);
        }
        else
        {
            spawnGlider(trafo.position, trafo.eulerAngles);
        }
		//aeroplaneController.Reset();
		//userControl.Reset();
	}


    public void spawnGlider(Vector3 position, Vector3 rotation)
    {
		transform.position=position;
		transform.eulerAngles=rotation;
        rigi.angularVelocity = Vector3.zero;
        Debug.Log("Spawning Glider at "+transform.position);
    }


		// Update is called once per frame
	void Update () {
		updateRespawn();
	}

    private void FixedUpdate()
    {
        if (transform.position.y > maxHeight)
        {
            rigi.AddForce(new Vector3(0, forceOnHeight, 0), ForceMode.Acceleration);
        }
        else
        {
            rigi.AddForce(forceByThermic, ForceMode.Acceleration);
        }
    }
}
