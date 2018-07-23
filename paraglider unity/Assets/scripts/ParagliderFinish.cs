using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParagliderFinish : MonoBehaviour {

	public Collider colli;
	public bool wantsPreviewTexture = true;
	public MeshRenderer previewQuad; //the quad that will show the render tecture
	public Transform dockingPoint;

	// Use this for initialization
	void Start () {
		colli = GetComponentInChildren<Collider>();
		colli.isTrigger=true;
		colli.enabled=false;
	}

	public void wake(RenderTexture previewTexture)
	{
		colli.enabled=true;
		if(previewQuad!=null)
		{
			if(previewTexture==null)
			{
				Debug.LogError("no preview texture received");
				previewQuad.enabled=false;
			}
			else
			{
				showTexture(previewTexture);
			}
		}
	}

	public void showTexture(RenderTexture previewTexture)
	{
		previewQuad.material.mainTexture = previewTexture;
		previewQuad.enabled=true;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
