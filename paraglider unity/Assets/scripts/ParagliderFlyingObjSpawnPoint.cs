using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParagliderFlyingObjSpawnPoint : MonoBehaviour {

	public Collider colli;
	// Use this for initialization
	void Start () {
		colli = gameObject.GetComponentInChildren<Collider>();
		colli.isTrigger = true;
		colli.name = "spawnCollider";
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
