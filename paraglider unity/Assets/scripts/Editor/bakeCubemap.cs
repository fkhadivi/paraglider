using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class bakeCubemap : ScriptableWizard
{
    public Transform renderFromPosition;
    public Cubemap cubemap;
	public Camera camera;
	public string name;
	public bool flipForSkymap=true;
	void OnWizardUpdate()

	{
	    //string helpString = "Select transform to render from and cubemap to render into";
	    bool isValid = (renderFromPosition != null) && (cubemap != null);
	}

	void OnWizardCreate()
	{
		if(cubemap==null)
	    {
	       	cubemap = new Cubemap(2048,TextureFormat.ARGB32,true);
	       	if(name==null)
	       	{
	       		name="_cubemap";
	       	}
			cubemap.name=name;
		}

	    // create temporary camera for rendering
	    GameObject go = new GameObject("CubemapCamera");
	    Camera gocam = go.AddComponent<Camera>();
			gocam.backgroundColor = camera.backgroundColor;
			gocam.clearFlags = camera.clearFlags;
			gocam.nearClipPlane= camera.nearClipPlane;
			gocam.farClipPlane= camera.farClipPlane;
			gocam.cullingMask = camera.cullingMask;
	    // place it on the object
	    if(renderFromPosition!=null)
	    {
	    	go.transform.position = renderFromPosition.position;
	    }
	    go.transform.rotation = Quaternion.identity;
	    // render into cubemap      
	    gocam.RenderToCubemap(cubemap);

	    // destroy temporary camera
	    DestroyImmediate(go);

	    ConvertToPng();
	}

	[MenuItem("GameObject/Render into Cubemap")]
	static void RenderCubemap()
	{
			ScriptableWizard.DisplayWizard<bakeCubemap>(
	        "Render cubemap", "Render!");
	}

	void ConvertToPng()
	{
	    Debug.Log(Application.dataPath + "/" +cubemap.name +"_PositiveX.png");
	    var tex = new Texture2D (cubemap.width, cubemap.height, TextureFormat.RGB24, false);
	    // Read screen contents into the texture        


		if(flipForSkymap)	
			tex.SetPixels(FlipPixelsVertically(cubemap.GetPixels(CubemapFace.PositiveX),cubemap.width,cubemap.height));
		else 
			tex.SetPixels(cubemap.GetPixels(CubemapFace.NegativeX));
	    var bytes = tex.EncodeToPNG();      
	    File.WriteAllBytes(Application.dataPath + "/"  + cubemap.name +"_left(+X).png", bytes);       

		if(flipForSkymap)	
			tex.SetPixels(FlipPixelsVertically(cubemap.GetPixels(CubemapFace.NegativeX),cubemap.width,cubemap.height));
		else 
			tex.SetPixels(cubemap.GetPixels(CubemapFace.NegativeX));
	    bytes = tex.EncodeToPNG();     
	    File.WriteAllBytes(Application.dataPath + "/"  + cubemap.name +"_right(-X).png", bytes);       

	    
		if(flipForSkymap)	
			tex.SetPixels(FlipPixelsVertically(cubemap.GetPixels(CubemapFace.PositiveY),cubemap.width,cubemap.height));
		else 
			tex.SetPixels(cubemap.GetPixels(CubemapFace.PositiveY));
	    bytes = tex.EncodeToPNG();     
	    File.WriteAllBytes(Application.dataPath + "/"  + cubemap.name +"_up(+y).png", bytes);       

	    
		if(flipForSkymap)	
			tex.SetPixels(FlipPixelsVertically(cubemap.GetPixels(CubemapFace.NegativeY),cubemap.width,cubemap.height));
		else 
			tex.SetPixels(cubemap.GetPixels(CubemapFace.NegativeY));
	    bytes = tex.EncodeToPNG();     
	    File.WriteAllBytes(Application.dataPath + "/"  + cubemap.name +"_down(-Y).png", bytes);       

	    
		if(flipForSkymap)	
			tex.SetPixels(FlipPixelsVertically(cubemap.GetPixels(CubemapFace.PositiveZ),cubemap.width,cubemap.height));
		else 
			tex.SetPixels(cubemap.GetPixels(CubemapFace.PositiveZ));
	    bytes = tex.EncodeToPNG();     
	    File.WriteAllBytes(Application.dataPath + "/"  + cubemap.name +"_front(+Z).png", bytes);       

	    
		if(flipForSkymap)	
			tex.SetPixels(FlipPixelsVertically(cubemap.GetPixels(CubemapFace.NegativeZ),cubemap.width,cubemap.height));
		else 
			tex.SetPixels(cubemap.GetPixels(CubemapFace.NegativeZ));
	    bytes = tex.EncodeToPNG();     
	    File.WriteAllBytes(Application.dataPath + "/"  + cubemap.name   +"_back(-Z).png", bytes);       

	    DestroyImmediate(tex);

	}

	Texture2D FlipPixelsVertically(Texture2D original)
 	{
    	Texture2D flipped = new Texture2D(original.width,original.height);

       for(int i=0;i<original.width;i++){
          for(int j=0;j<original.height;j++){
              flipped.SetPixel(original.width,original.height-1-j, original.GetPixel(i,j));
          }
       }
        flipped.Apply();
     
        return flipped;
    }

	Color[] FlipPixelsVertically(Color[] original,int width,int height)
 	{
    	Color[] flipped = new Color[width*height];

       for(int y=0;y<height;y++){
          for(int x=0;x<width;x++){
              flipped[(height-y-1)*width+x] = original[y*width+x];
          }
       }
       return flipped;
    }
}