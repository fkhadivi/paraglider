using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class remove : MonoBehaviour {

	public bool removeNow = false;
	public bool disableOnly = false;
	public Material[] Materials;
	public Transform target;
	public float maxDistance = 9999;
	public float afterTime = 1;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(removeNow)
		{
			int i;
			for (i=0;i<Materials.Length;i++)
			{
				//Debug.Log(GetComponent<MeshRenderer>().sharedMaterial +" =? "+Materials[i]);
				if(GetComponent<MeshRenderer>().sharedMaterial == Materials[i])
				{
					doIt();
					return;
				}
			}
			if( maxDistance>0 && target!=null && Vector3.Distance(target.position, transform.position)<maxDistance)
			{
				doIt();
				return;
			}
			if(afterTime>0)
			{
				afterTime-=Time.deltaTime;
				if(afterTime<=0)
				{
					doIt();
					return;
				}
			}
			

		}
	}

	void doIt()
	{
		if(disableOnly) this.gameObject.SetActive(false)     ;
		else Destroy(this.gameObject);
		Debug.Log(this.name+"Has been removed");
	}
		
}
