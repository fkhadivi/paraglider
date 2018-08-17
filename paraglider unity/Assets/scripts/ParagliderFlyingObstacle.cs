using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParagliderFlyingObstacle : MonoBehaviour {

	public Transform player;
	public float speed = 0.0f;
	public bool agrassive=true;
	public Color debugColor;
	public Rigidbody rigi;
	public float maximumVelocityInKMH = 50;
	public float maximumAccelerationInG = 1;


	// Use this for initialization
	void Start () {
		player = findGlider();
		debugColor = new Color(Random.value,Random.value,Random.value);
        rigi = gameObject.GetComponent<Rigidbody>();
        if (rigi == null)
			rigi = gameObject.AddComponent<Rigidbody>();
		rigi.useGravity=false;
        //rigi.isKinematic = true;
		rigi.mass = 1000;
	}

    private void OnEnable()
    {
        Start();
    }

    Transform findGlider()
	{
		Transform glider = null;
		AeroplaneUserControl player = transform.root.GetComponentInChildren<AeroplaneUserControl>();
		Debug.Log("ae user search: " + player);
		if(player != null) 
			glider = player.transform;
		else 
			glider = GameObject.Find("Glider").transform;	
		if (glider == null) 
			glider = GameObject.Find("glider").transform;
		if (glider == null) 
			Debug.LogError("Glider could not be found ... I feel lonely, where is my human player?");
		else Debug.Log("Glider found by name of "+glider.name+" ... Hello human, let's ... PLAY");
		return glider;
	}



	void interceptRectangular (Transform vessel,float maxVelo,float maxAcc)
	{
		if(maxVelo != 0 && maxAcc != 0)
		{
			//calculate direction from this object into forward path of glider
			Vector3 direction = Vector3.Cross(vessel.forward,vessel.position-transform.position);
			direction = Vector3.Cross(direction,vessel.forward);

			//calculate point of rendevous (intersection of flightpaths)
			Vector3 rendevous= BenjasMath.ClosestPointToIntersect(vessel.position,vessel.forward,transform.position,direction);
			//has player passed the point of a 90° intercept course?
			Vector3 rendevousVessel = rendevous-vessel.position;
			bool passed = Vector3.Dot(rendevousVessel,vessel.forward)<0;
			if(!passed)
			{
				Debug.DrawLine(transform.position,rendevous,debugColor);
				Debug.DrawLine(vessel.position,rendevous);

				// where do we need to fly
				direction=(rendevous-transform.position);

				// Timing
				// velocity player
				float velo = player.GetComponent<Rigidbody>().velocity.magnitude;
				// estimated Time of arrival of player
				float timeVessel = rendevousVessel.magnitude/Mathf.Max(velo,0.000001f);
				// estimated Time of arrival of this
				float time = direction.magnitude/Mathf.Max(rigi.velocity.magnitude,0.000001f);

					//use direction for acceleration
				direction = direction.normalized * maxAcc;
					//acc forward or backward
				direction *=(time>timeVessel)?1:-1;
					//softly restrict speed to 'maximumVelocity' 	
				direction *= (1-rigi.velocity.magnitude/maxVelo);
					//apply acceleration
				rigi.AddForce(direction,ForceMode.Acceleration);
				speed = rigi.velocity.magnitude;
			}
		}
		//
	}
			

	public float MaxSpawningAngle=30;
	public float MinSpawningAngle=10;
	public bool getAnglefromCam = true;
	public float spawnDist = 200;
	public float minHeight = 50;
	public float maxHeight = 150;
	public float MaxTimeToSpawn = 0.5f;
	public bool doSpawn = false;
	public float TimeToSpawn = 0;
	bool spawnInFromleft = false;
	bool spawning=false;


	public void spawn()
	{
		if(!spawning)
		{
			//start spawning
			spawning=true;

			Rigidbody playerRigi = player.GetComponent<Rigidbody>();

			//set spawning angle (angle relative to forward view) 
			if(getAnglefromCam) MaxSpawningAngle = horrizontalFOV(Camera.main);
			//if not make sure Max spawning angle is positive = right side of screen
			else MaxSpawningAngle=Mathf.Abs(MaxSpawningAngle);
			// in any case set Min angle to positiv
			MinSpawningAngle = Mathf.Abs(MinSpawningAngle);
			// than check if player is turning left and if so, make angles negative = left side of screen 
			if(playerRigi.angularVelocity.y<0) 
			{
				MaxSpawningAngle=-MaxSpawningAngle;
				MinSpawningAngle=-MinSpawningAngle;
			}
			TimeToSpawn = MaxTimeToSpawn;
		}

		TimeToSpawn -= Time.deltaTime;

		if (TimeToSpawn<0)
		{
			//stop spawning
			TimeToSpawn = 0;
			spawning=false;
		}
		//now place this object depneding on time
		//find position before the player
		Vector3 pos = player.forward ;
		//put it in the horrizontal plane
		pos.y=0; 
		//apply distance
		pos *= spawnDist;
		//rotate around player
		pos = Quaternion.Euler(0,Mathf.Lerp(MinSpawningAngle,MaxSpawningAngle,TimeToSpawn/MaxTimeToSpawn),0) * pos;
		//find the spot
		pos+=player.position;
		//clapm it
		pos.y= Mathf.Clamp(pos.y,minHeight,maxHeight);
		transform.position=pos;
		//set velocity to 0 yust in case
		rigi.velocity = new Vector3();
	}

	//return the horrizontal Field of View of a camera
	 float horrizontalFOV(Camera cam){

		float degrees = cam.fieldOfView ;
		degrees *= Mathf.Deg2Rad;
		degrees = 2f * Mathf.Atan(Mathf.Tan(degrees / 2f) * cam.aspect);
		degrees *= (float) Mathf.Rad2Deg;
		return degrees;

	}

	void Update () 
	{
        transform.localEulerAngles = Vector3.zero;

		if(player!= null)
		{
			if(spawning || doSpawn )
			{
				doSpawn = false;
				spawn();
			}
		}
	}

	// Update is called once per frame
	void FixedUpdate () 
	{

		if(player!= null)
		{
			if(!spawning && maximumVelocityInKMH!=0)
			{
			//	interceptRectangular(player,maximumVelocityInKMH/3.6f,maximumAccelerationInG*9.81f);
			}
		}
	}

}

