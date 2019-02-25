using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class billboardBkShaderRotation : MonoBehaviour {

    private Material Matty;
    private bool hasBillboardShader = false;
    private string shaderNames = "Unlit/billboardBK, Unlit/billboardYonlyBK";
    private float rotationOffset = 0f;

    // Use this for initialization
    void Start () {
        Matty = GetComponent<MeshRenderer>().sharedMaterial;
		if(Matty ==null)
		{
			Debug.LogError("no Material on " + gameObject.name+"'. This script needs one of the shaders '"+ shaderNames+"' to make it a billboard");
		}
		else if (!shaderNames.Contains(Matty.shader.name))
        {
			Debug.LogWarning("Wrong shader in " + gameObject.name+ ": the Material "+ Matty.name + " uses the shader '"+ Matty.shader.name +"'. This script needs one of the shaders '"+ shaderNames+"' to make it a billboard");
        }
        else
        {
			hasBillboardShader = true;
            Matty = GetComponent<MeshRenderer>().material;
            rotationOffset = Matty.GetFloat(Shader.PropertyToID("_Rotation"));
        }
    }

    // Update is called once per frame
    void Update () {
        if(hasBillboardShader) Matty.SetFloat(Shader.PropertyToID("_Rotation"), transform.eulerAngles.z+rotationOffset);

    }
}
