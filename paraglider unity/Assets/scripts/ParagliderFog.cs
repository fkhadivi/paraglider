using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParagliderFog : MonoBehaviour {

	public Collider Colli;

	// Use this for initialization
	void Start () {
		Colli = gameObject.GetComponentInChildren<Collider>();
		Colli.isTrigger=true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
