using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParagliderCrashDetection : MonoBehaviour {

	public string finishColliderName = "finishCollider";
	public string crashColliderName = "crashCollider";
	public string fogColliderName = "fogCollider";

	public delegate void Delegate();
	public Delegate onCrash;
	public Delegate onFog;
	public Delegate onFinish;

	public float respawnTimePanalalty = 5f;
    public float respawnTimer = 0;
	public float respawnHeight = 180f;
    public List<Vector3> respawnCollector = new List<Vector3>();


	// Use this for initialization
	void Start () {
	}

	void OnCollisionEnter(Collision collision)
    {
		onCrash();
    }

    void OnTriggerEnter(Collider col)
    {
		if (col.gameObject.name == finishColliderName) onFinish();
		else if (col.gameObject.name == fogColliderName) onFog();
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
		Vector3 position=respawnCollector[0];
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

	public void spawnGlider(Transform trafo)
	{
		spawnGlider(trafo.position,trafo.eulerAngles);
	}


    public void spawnGlider(Vector3 position, Vector3 rotation)
    {
		transform.position=position;
		transform.eulerAngles=rotation;
		Debug.Log("Spawning Glider at "+transform.position);
    }


		// Update is called once per frame
	void Update () {
		updateRespawn();
	}
}
