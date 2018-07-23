using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFourPointProjection : MonoBehaviour {

	public Camera theCam;
    public Transform screenQuad;
    public bool useScreenQuadCorners = true;
	


	public Transform topLeft;
	public Transform topRight;
	public Transform bottomLeft;
	public Transform bottomRight;
    public bool drawNearCone, drawFrustum, drawscreenQuad;
    public bool adjustClippingPlanes = false;
    Matrix4x4 p0;
    public float factor = 0;
    public bool keepUpdatingCam = false;
    public bool showscreenQuad = false;
    public bool makeSnapshots = false;
    public bool showDebug = false;


    public void Setup()
    {
		p0 = theCam.projectionMatrix;
		screenRendi = screenQuad.gameObject.GetComponent<MeshRenderer>();
        screenRendi.enabled = false;
    	if(useScreenQuadCorners)
    	{
    		//generate 4 new game objects and put them ito the quads corners	

			placeAt(ref topLeft, "topLeft", new Vector3(-0.5f,	+0.5f, 0));
			placeAt(ref topRight, "topRight", new Vector3(+0.5f,	+0.5f, 0));	
			placeAt(ref bottomLeft, "bottomLeft", new Vector3(-0.5f,	-0.5f, 0));			
			placeAt(ref bottomRight, "bottomRight", new Vector3(+0.5f,	-0.5f, 0));				
		}
		updateCam();
    }

    Transform placeAt(ref Transform corner, string name, Vector3 pos)
    {
		if(corner == null)
		{
			//make a new corner
			corner = new GameObject().transform;
		}
		corner.name = name;
		corner.parent=screenQuad;
		corner.localScale= Vector3.one;
		corner.localEulerAngles = Vector3.zero;
		corner.localPosition = pos;
		return corner;
    }

	MeshRenderer screenRendi;  

	public void updateCam()
	{
		theCam.projectionMatrix = projetcionMatrix();

		screenRendi.enabled = showscreenQuad;
	}

	public void setScreenTexture (Texture2D tex)
	{
		screenRendi.material.mainTexture= tex;
	}

	public void setScreenTexture (RenderTexture tex)
	{
		screenRendi.material.mainTexture= tex;
	}

	void Update()
    {
    	if (keepUpdatingCam)
    		updateCam();

        if (makeSnapshots)
	        StartCoroutine(snapshot());
    }

	public Matrix4x4 projetcionMatrix()
	{
		Vector3 pa, pb, pc, pd;
		pa = bottomLeft.position; //Bottom-Left
		pb = bottomRight.position; //Bottom-Right
		pc = topLeft.position; //Top-Left
		pd = topRight.position; //Top-Right

        Vector3 pe = theCam.transform.position;// eye position

        Vector3 vr = (pb - pa).normalized; // right axis of screenQuad
        Vector3 vu = (pc - pa).normalized; // up axis of screenQuad
        Vector3 vn = Vector3.Cross(vr, vu).normalized; // normal vector of screenQuad

        Vector3 va = pa - pe; // from pe to pa
        Vector3 vb = pb - pe; // from pe to pb
        Vector3 vc = pc - pe; // from pe to pc
        Vector3 vd = pd - pe; // from pe to pd

        float n = -screenQuad.InverseTransformPoint(theCam.transform.position).z; // distance to the near clip plane (screenQuad)
        float f = theCam.farClipPlane; // distance of far clipping plane
        float d = Vector3.Dot(va, vn); // distance from eye to screenQuad
        float l = Vector3.Dot(vr, va) * n / d; // distance to left screenQuad edge from the 'center'
        float r = Vector3.Dot(vr, vb) * n / d; // distance to right screenQuad edge from 'center'
        float b = Vector3.Dot(vu, va) * n / d; // distance to bottom screenQuad edge from 'center'
        float t = Vector3.Dot(vu, vc) * n / d; // distance to top screenQuad edge from 'center'

        Matrix4x4 p = p0; //Projection matrix
        p[0, 0] = 2.0f * n / (r - l);
        p[0, 2] = (r + l) / (r - l);
        p[1, 1] = 2.0f * n / (t - b);
        p[1, 2] = (b + t) / (b - t);
        

        p[2, 2] = (f + n) / (n - f);
        if (adjustClippingPlanes) p[2, 3] = 2.0f * f * n / (n - f);
        p[3, 2] = -1.0f;

		if (showDebug) {

			if (drawNearCone)
	        { //Draw lines from the camera to the corners f the screenQuad
	            Debug.DrawRay(theCam.transform.position, va, Color.blue);
	            Debug.DrawRay(theCam.transform.position, vb, Color.blue);
	            Debug.DrawRay(theCam.transform.position, vc, Color.blue);
	            Debug.DrawRay(theCam.transform.position, vd, Color.blue);
	        }


	        if (drawscreenQuad)
	        { //Draw lines from the camera to the corners f the screenQuad
	            Debug.DrawLine(topLeft.position, topRight.position, Color.green);
				Debug.DrawLine(topRight.position, bottomRight.position, Color.green);
				Debug.DrawLine(bottomRight.position, bottomLeft.position, Color.green);
				Debug.DrawLine(bottomLeft.position, topLeft.position, Color.green);
	        }
	    
        	if (drawFrustum) DrawFrustum(theCam); //Draw actual camera frustum

        }
		return p;
	}


    Vector3 ThreePlaneIntersection(Plane p1, Plane p2, Plane p3)
    { //get the intersection point of 3 planes
        return ((-p1.distance * Vector3.Cross(p2.normal, p3.normal)) +
                (-p2.distance * Vector3.Cross(p3.normal, p1.normal)) +
                (-p3.distance * Vector3.Cross(p1.normal, p2.normal))) /
            (Vector3.Dot(p1.normal, Vector3.Cross(p2.normal, p3.normal)));
    }

    void DrawFrustum(Camera cam)
    {
        Vector3[] nearCorners = new Vector3[4]; //Approx'd nearplane corners
        Vector3[] farCorners = new Vector3[4]; //Approx'd farplane corners
        Plane[] camPlanes = GeometryUtility.CalculateFrustumPlanes(cam); //get planes from matrix
        Plane temp = camPlanes[1]; camPlanes[1] = camPlanes[2]; camPlanes[2] = temp; //swap [1] and [2] so the order is better for the loop

        for (int i = 0; i < 4; i++)
        {
            nearCorners[i] = ThreePlaneIntersection(camPlanes[4], camPlanes[i], camPlanes[(i + 1) % 4]); //near corners on the created projection matrix
            farCorners[i] = ThreePlaneIntersection(camPlanes[5], camPlanes[i], camPlanes[(i + 1) % 4]); //far corners on the created projection matrix
        }

        for (int i = 0; i < 4; i++)
        {
            Debug.DrawLine(nearCorners[i], nearCorners[(i + 1) % 4], Color.red, Time.deltaTime, false); //near corners on the created projection matrix
            Debug.DrawLine(farCorners[i], farCorners[(i + 1) % 4], Color.red, Time.deltaTime, false); //far corners on the created projection matrix
            Debug.DrawLine(nearCorners[i], farCorners[i], Color.red, Time.deltaTime, false); //sides of the created projection matrix
        }
    }

    // Take a shot immediately
    IEnumerator snapshot()
    {

        // We should only read the screenQuad buffer after rendering is complete
        yield return new WaitForEndOfFrame();

        RenderTexture.active = theCam.targetTexture;
        Texture2D tex = new Texture2D(theCam.targetTexture.width, theCam.targetTexture.height);
        
        tex.ReadPixels(new Rect(0, 0, theCam.targetTexture.width, theCam.targetTexture.height), 0, 0);
        tex.Apply();


        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();
        Object.Destroy(tex);
        System.IO.File.WriteAllBytes(Application.dataPath + "/../"+this.name+"_"+ Time.frameCount.ToString()+ ".png", bytes);
    }
}
