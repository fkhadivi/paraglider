using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class paragliderFogLayer : MonoBehaviour {

	private Material mat;
	public Color fogColor = Color.white;
	public float maxFogTime = 10;
	private float fogTime =0;
	private bool isFogging = false;
	public bool testFog = false;
	// Use this for initialization
	void Start () {
		mat = gameObject.GetComponent<MeshRenderer>().material;
		setAlpha(0.0f);
	}

	public void doFog()
	{
		fogTime=maxFogTime;
		isFogging = true;
		testFog = false;
	}

	void setAlpha (float alpha)
	{
		fogColor.a=Mathf.Clamp01(alpha);
		mat.SetColor("_Color",fogColor);
	}

	float timeFunktion(float t)
	{
		return (1-Mathf.Cos(6.283f*t))/2;
	}
	
	// Update is called once per frame
	void Update () {
		if(testFog)
		{
			doFog();
		}
		if(isFogging)
		{
			if(BenjasMath.countdownToZero(ref fogTime))
			{
				isFogging = false;
			}
			setAlpha(20*timeFunktion(fogTime/maxFogTime));
		}
	}
}
