using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAppearence_benja : MonoBehaviour {

public bool changeNow = false;

public Material[] materials;
public bool changeMaterials = false;
private MeshRenderer rendi;

public Mesh[] meshes;
public bool changeMeshes = false;
private MeshFilter meshi;

	// Use this for initialization
	void Start () {
		rendi=this.gameObject.GetComponent<MeshRenderer>();
		meshi = this.gameObject.GetComponent<MeshFilter>();
	}


	int randomInt (float i)
	{
		return (int) Mathf.Floor(Random.value*(float)(i+0.999999f));
	}

	// Update is called once per frame
	void Update () {
	if(changeNow)
	{
		changeNow=false;
		if(changeMeshes && meshes.Length>1)
		{
			int i = randomInt(meshes.Length-1);
			if(meshes[i]!=null)
			{
				meshi.mesh=meshes[i];
			}
		}
		if(changeMaterials && materials.Length>1)
		{
			int i = randomInt(materials.Length-1);
			if(materials[i]!=null)
			{
				rendi.material=materials[i];
			}
		}
	}
		
	}
}
