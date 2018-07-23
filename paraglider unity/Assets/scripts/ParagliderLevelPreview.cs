using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParagliderLevelPreview : MonoBehaviour {

	public cameraFourPointProjection projection;
	public ParagliderFinish previewFinishObject; //docking point from finish object!
	public RenderTexture previewTexture;
	public Vector2Int RendertextureScale = new Vector2Int(4096,4096);
	public bool renderOneFrame = false;
	bool isSetUp = false;
	// Use this for initialization
	void Start () {
		//setup the projection of the camera to the target quad to get an exact projetion that can be reused by the level frame
		previewTexture = new RenderTexture(RendertextureScale.x,RendertextureScale.y,0);
		if(previewFinishObject==null)
		{
			previewFinishObject = GetComponentInChildren<ParagliderFinish>();
		}
		//now adjust the camera position
		previewFinishObject.transform.parent=projection.theCam.transform.parent;
		projection.theCam.transform.position = previewFinishObject.dockingPoint.position;
		previewFinishObject.transform.parent=projection.theCam.transform;

		//now do the setup of the projection
		projection.screenQuad = previewFinishObject.previewQuad.transform;
		projection.Setup();
		projection.theCam.targetTexture = previewTexture;
		projection.theCam.enabled=false;
		isSetUp=true;
	}


	public RenderTexture getPreviewTexture()
	{
		if(!isSetUp)
			Start();
		projection.theCam.enabled=true;
		previewFinishObject.gameObject.SetActive(false);
		projection.updateCam();
		projection.theCam.Render();
		projection.theCam.enabled=false;
		previewFinishObject.gameObject.SetActive(true);
		previewFinishObject.showTexture(previewTexture);
		//projection.setScreenTexture(previewTexture);
		return previewTexture;
	}

	// Update is called once per frame
	void update() {
		if (renderOneFrame)
		{
			renderOneFrame=false;
			getPreviewTexture();
		}
	}
}
