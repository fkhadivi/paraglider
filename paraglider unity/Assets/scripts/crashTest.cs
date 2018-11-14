using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crashTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        rigi = GetComponent<Rigidbody>();
        
	}

    public Rigidbody rigi;

    // Update is called once per frame
    void Update () {
        Debug.DrawLine(transform.position, transform.position + rigi.velocity, Color.magenta);
        veloBackup = velo;
        velo = rigi.velocity;

    }

    public float crashAngle(Collision collision)
    {
        return Vector3.Angle(collision.relativeVelocity, collision.contacts[0].normal);
    }

    public float angle;
    public Vector3 velo;
    public Vector3 veloBackup;
    public Vector3 veloOnCrash;
    public Vector3 veloRel;

    public Vector3 normal;
    private void OnCollisionEnter(Collision collision)
    {
        
        normal = collision.contacts[0].normal;
        angle = crashAngle(collision);
        veloOnCrash = rigi.velocity;
        veloRel = collision.relativeVelocity;
        Debug.DrawLine(transform.position, transform.position + collision.contacts[0].normal.normalized*999, Color.green);
        Debug.DrawLine(transform.position, transform.position + collision.relativeVelocity * 999, Color.blue);
        Debug.Break();
    }
}
