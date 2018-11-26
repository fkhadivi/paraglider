using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class billboardCPU : MonoBehaviour {

    public GameObject target;
    [Header("billboard around")]
    public bool xAxis = true;
    public bool yAxis = true;
    public bool zAxis = true;
    private Vector3 eulers;
    private Vector3 current;
    private bool isSetUp = false;

    // Use this for initialization
    void Setup () {
        eulers = transform.localEulerAngles;
        if (target == null)
        {
            target = Camera.main.gameObject;
            Debug.Log(Camera.main.gameObject.name + "found as cam");
        }
	}


	
	// Update is called once per frame
	void Update () {
        if (!isSetUp) Setup();
        if (xAxis || yAxis || zAxis)
        {

            //transform.LookAt(target.transform);
            transform.eulerAngles = target.transform.eulerAngles;
            current = transform.localEulerAngles;
            if (!xAxis) current.x = eulers.x; else current.x += 0;
            if (!yAxis) current.y = eulers.y; else current.y += 0;
            if (!zAxis) current.z = eulers.z; else current.z +=0;
            transform.localEulerAngles = current;
        }

    }
}
