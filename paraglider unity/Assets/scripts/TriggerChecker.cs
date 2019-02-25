using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerChecker : MonoBehaviour {


    public delegate void Deli(Collider other);
    public Deli onEnter;
    public Deli onExit;
    public Deli onStay;
    public bool changeOnTrigger = false;

    
	// Use this for initialization
	void Start () {
        GetComponent<Collider>().isTrigger = true;
        //Debug.Log(" ███ trigger adjusted");
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.LogWarning(" ███ enter trigger " + other.name);
        changeOnTrigger = !changeOnTrigger;
        if (onEnter != null)
        {
            onEnter(other);
        }
        else
        {
            if(onEnter == null && onExit == null && onStay == null)
            Debug.LogWarning("no function attached to onTrigger");
        }
    }

    void OnTriggerExit(Collider other)
    {
        //Debug.LogWarning(" ███ exit trigger " + other.name);
        changeOnTrigger = !changeOnTrigger;
        if (onExit != null)
        {
            onExit(other);
        }
        else
        {
            if (onEnter == null && onExit == null && onStay == null)
                Debug.LogWarning("no function attached to onTrigger");
        }
    }


    private void OnTriggerStay(Collider other)
    {
        //Debug.LogWarning(" ███ stay trigger " + other.name);
        changeOnTrigger = !changeOnTrigger;
        if (onStay != null)
        {
            onStay(other);
        }
        else
        {
            if (onEnter == null && onExit == null && onStay == null)
                Debug.LogWarning("no function attached to onTrigger");
        }
    }

}
