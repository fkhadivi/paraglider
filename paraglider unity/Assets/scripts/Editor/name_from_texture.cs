using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class name_from_texture : MonoBehaviour {


    public MeshRenderer meshRenderer;
    public bool prepareAsParagliderObject = false;

	// Use this for initialization
	void Start () {
        if (prepareAsParagliderObject)
        {
            prepareAsParagliderObject = false;
            gameObject.name = GetComponent<MeshRenderer>().sharedMaterial.name;
            if (true || transform.parent.gameObject.name == "alle objekte") makeMutti();
            if (gameObject.transform.parent.gameObject.GetComponentInChildren<BoxCollider>() == null) makeColli();
        }
    }


    void makeColli()
    {
        GameObject crashCollider = new GameObject();
        crashCollider.transform.position = gameObject.transform.position;
        crashCollider.transform.rotation = gameObject.transform.rotation;
        crashCollider.transform.parent = gameObject.transform;
        crashCollider.transform.localScale = new Vector3(1, 1, 0.5f);
        crashCollider.transform.parent = gameObject.transform.parent;
        crashCollider.name = "crashCollider";
        crashCollider.AddComponent<BoxCollider>();
    }

	void makeMutti () {
        GameObject mammy = new GameObject();
        mammy.transform.position = gameObject.transform.position;
        mammy.transform.rotation = gameObject.transform.rotation;
        mammy.transform.parent = gameObject.transform;
        mammy.transform.parent = gameObject.transform.parent;
        //mammy.transform.localScale = new Vector3(1, 1, 1);
        gameObject.transform.parent = mammy.transform;
        mammy.name = gameObject.name;
    }
}
