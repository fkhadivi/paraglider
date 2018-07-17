using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigWheel_benja : MonoBehaviour {

	public Transform wheel;
	public Transform[] Gondolas;
	public float speed= 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 rot = new Vector3(0,0,Time.deltaTime * speed);
		wheel.gameObject.transform.Rotate(-rot);
		for (int i = 0; i < Gondolas.Length; i++) 
		{
			Gondolas [i].gameObject.transform.Rotate (rot);
		}
	}
}
