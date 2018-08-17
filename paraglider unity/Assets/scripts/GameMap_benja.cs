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

    public Vector3 NEworld;
    public Vector3 SWworld;
	//RectTransform NEmap;
	//RectTransform SWmap;
	public Vector2 mapMin = new Vector2(-0.5f,-0.5f);
	public Vector2 mapMax = new Vector2(0.5f,0.5f);
    public Vector2 mapMinScaled ;
    public Vector2 mapMaxScaled ;
    public Vector2 clampMin;
    public Vector2 clampMax;
    public Vector2 mapScale;

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

        RectTransform maptrafo = mapImage.GetComponent<RectTransform>();
        Vector2 mapScale = new Vector2(maptrafo.rect.width, maptrafo.rect.height);
        mapMinScaled = mapMin * mapScale;
        mapMaxScaled = mapMax * mapScale;
        /*		Vector2 pointerScale = new Vector2(mapPointer.rect.width,mapPointer.rect.height);
        clampMin = mapMinScaled + 0.5f * pointerScale;
		clampMax = mapMaxScaled - 0.5f * pointerScale;
        */
        clampMin = mapMinScaled ;
		clampMax = mapMaxScaled ;
	}

	private void updateMap(Vector3 coordinates)
	{
        
		positionMap = mapPointer.anchoredPosition3D;
		positionWorld = coordinates;

        //Debug.Log("map" +positionMap.x);

		positionMap.x=BenjasMath.map(	positionWorld.x,
							SWworld.x,NEworld.x,
                            mapMinScaled.x, mapMaxScaled.x,
							clampMin.x,clampMax.x);
        //Debug.Log(positionMap.x);
        positionMap.y=BenjasMath.map(	positionWorld.z,
							SWworld.z,NEworld.z,
                            mapMinScaled.y, mapMaxScaled.y,
							clampMin.y,clampMax.y);

		mapPointer.anchoredPosition3D = positionMap;
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



}
