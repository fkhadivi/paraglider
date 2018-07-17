using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParagliderStaticObstackles : MonoBehaviour {

		public GameObject[] spawns;
		public int maxCount;
		public bool spawnsSearched=false;

	     public void findSpawns()
	     {
			spawns = new GameObject[transform.childCount];
			for (int ID = 0; ID < transform.childCount; ID++)
	        {
			spawns [ID] = transform.GetChild(ID).gameObject;
	        }
	     }

		public int distribute(int maxAppearances)
		{
			if(!this.spawnsSearched)
			findSpawns();
			maxCount=maxAppearances;


			Debug.Log(name+" generating random indices");
			BenjasMath.randomizeArray(spawns);
			for(int i=0; i<spawns.Length; i++)
			{
				spawns[i].SetActive((i < maxAppearances));
			}
			return maxAppearances;
		}



	

	// Use this for initialization
	void Start () {
		findSpawns();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
