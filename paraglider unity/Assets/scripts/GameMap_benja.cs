using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMap_benja : MonoBehaviour {

	public Image mapImage;
	public RectTransform mapPointer;
	// Use this for initialization


	void Start () {
		
	}

	Vector3 NEworld;
	Vector3 SWworld;
	//RectTransform NEmap;
	//RectTransform SWmap;
	public Vector2 mapMin = new Vector2(-0.5f,-0.5f);
	public Vector2 mapMax = new Vector2(0.5f,0.5f);
	Vector2 clampMin;
	Vector2 clampMax;
	public Vector3 positionWorld;
	public Vector3 rotationWorld;
	public Vector3 positionMap;
	public Vector3 rotationMap;

	public Vector2 position = new Vector2();
	public float rotation = 0;


	// assuming the world beeing x = east west, y = altitude and z= north south
	// so put in the most northeast and most southwest points in the real world covered by the map 
	// to enable the map to map correctly 
	// also provide the map data as a texture2D
	// and now ask your colleges if they want a coffee too
	public void Setup(Sprite MapTexture,Vector3 northeast, Vector3 southwest)
	{
		mapImage.sprite = MapTexture;
		NEworld = northeast;
		SWworld = southwest;
		Vector2 scale = new Vector2(mapPointer.localScale.x/2,mapPointer.localScale.y/2);
		clampMin = mapMin+scale;
		clampMax = mapMax-scale;
	}

	public void updateMap(Vector3 coordinates)
	{
		positionMap = mapPointer.localPosition;
		positionWorld = coordinates;


		positionMap.x=map(	positionWorld.x,
							SWworld.x,NEworld.x,
							mapMin.x,mapMax.x,
							clampMin.x,clampMax.x);

		positionMap.y=map(	positionWorld.z,
							SWworld.z,NEworld.z,
							mapMin.y,mapMax.y,
							clampMin.y,clampMax.y);

		mapPointer.localPosition = positionMap;
	}

	public void updateMap(Vector3 coordinates, Vector3 direction)
	{
		updateMap(coordinates);
		rotationWorld =direction;
		rotationMap = mapPointer.localRotation.eulerAngles;
		rotationMap.z = -direction.y;
		mapPointer.localRotation = Quaternion.Euler(rotationMap);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Mapping
	float map(float value, float minIn, float maxIn, float minOut,float maxOut)
	{
		return 	Mathf.Lerp(minOut,maxOut,Mathf.InverseLerp(minIn,maxIn,value));
	}

	float map(float value, float minIn, float maxIn, float minOut,float maxOut, float minClamp, float maxClamp)
	{
			return 	Mathf.Clamp(map(value,minIn,maxIn,minOut,maxOut),minClamp,maxClamp);
	}

}
