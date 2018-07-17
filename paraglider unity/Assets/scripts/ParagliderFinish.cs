using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParagliderFinish : MonoBehaviour {

	public Collider colli;

	// Use this for initialization
	void Start () {
		colli = GetComponentInChildren<Collider>();
		colli.isTrigger=true;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
