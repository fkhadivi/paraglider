using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dublicate : MonoBehaviour {

	public Vector3Int number = new Vector3Int(1,1,1);
	public Vector3 offset;
	public bool scaleToOffset = false;
	public bool update = false;
	public GameObject[] clones;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(scaleToOffset)
		{
			offset = gameObject.transform.lossyScale;
		}
		if(update)
		{
			update=false;
			int i;
			for(i=0;i<clones.Length;i++)
			{
				Destroy(clones[i]);
			}
			i=number.x*number.y*number.z;
			if (i>0)
			{
				clones = new GameObject[i];
				i=0;
				for(int x=0;x<number.x;x++)
				for(int y=0;y<number.y;y++)
				for(int z=0;z<number.z;z++)
				{
					if(x+y+z>0)
					{
						clones[i] = Instantiate(gameObject,
						gameObject.transform.position+new Vector3(offset.x*x,offset.y*y,offset.z*z),
						gameObject.transform.rotation,
						gameObject.transform.parent);
						clones[i].name = gameObject.name+" "+x+" "+y+" "+z;
						Destroy(clones[i].GetComponent<dublicate>());
						i++;
					}
				}
			}
		}
	}
}
