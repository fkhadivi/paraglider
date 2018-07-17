using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class renderTextureSnapshot : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public bool takeSnapshot;
    public Camera theCam;
    // Update is called once per frame
    void Update () {
        if (takeSnapshot)
        {
            StartCoroutine(snapshot());
            takeSnapshot = false;
        }

    }



// Take a shot immediately
IEnumerator snapshot()
{

    // We should only read the screen buffer after rendering is complete
    yield return new WaitForEndOfFrame();

    RenderTexture.active = theCam.targetTexture;
    Texture2D tex = new Texture2D(theCam.targetTexture.width, theCam.targetTexture.height);

    tex.ReadPixels(new Rect(0, 0, theCam.targetTexture.width, theCam.targetTexture.height), 0, 0);
    tex.Apply();


    // Encode texture into PNG
    byte[] bytes = tex.EncodeToPNG();
    Object.Destroy(tex);
    System.IO.File.WriteAllBytes(Application.dataPath + "/../" + this.name + "_" + Time.frameCount.ToString() + ".png", bytes);
}
}
