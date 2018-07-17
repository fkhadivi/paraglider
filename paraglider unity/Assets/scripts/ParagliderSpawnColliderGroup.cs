using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParagliderSpawnColliderGroup : MonoBehaviour {

		public ParagliderFlyingObjSpawnPoint[] spawns;

	     public void findSpawns()
	     {
		spawns = gameObject.GetComponentsInChildren<ParagliderFlyingObjSpawnPoint>();
	     }

	// Use this for initialization
	void Start () {
		findSpawns();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
