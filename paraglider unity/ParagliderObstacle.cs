using UnityEngine;
using System.Collections;


public class ParagliderObstacle : ScriptableObject
{
	public string Name;
	public GameObject gameObject;
	public int[] copies = new int[4];
	public float[] speed = new float[4];
	public bool[] appear = new bool[4];
}

