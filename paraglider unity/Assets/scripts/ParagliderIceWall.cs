using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParagliderIceWall : MonoBehaviour {

	Animator Ani;
	public bool triggered=false;
	private bool broken=false;
	public bool doReset = false;

    void Start()
    {
		Ani = gameObject.GetComponent<Animator>();
		Reset();
	}



	public void Reset()
	{
		triggered = false;
		doReset = false;
	}


	// Update is called once per frame
	void Update () {
		if (doReset)
			Reset ();
		
		if(triggered != broken)
		{
			broken=triggered;
			Ani.SetBool("broken",broken);
		}
		test=Ani.GetBool("broken");
	}

	public bool test=false;

	void OnTriggerEnter (Collider col)
	{
		if(!triggered)
		{
			triggered = true;
			Debug.Log (col.gameObject.name +"detected, icewall triggered!");
		}
	}
}
