using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

//[ExecuteInEditMode]
public class ParagliderLevel : MonoBehaviour {

	 
	public string level;
	[Header("Terrain Settings")]
	public GameObject terrain;
	public GameObject startPoint;
	public ParagliderFinish finish;
	public GameObject markerNE;
	public GameObject markerSW;

	public Texture2D map;
	public Vector2 scale;
	//public List<Obstacle> obstacles = new List<Obstacle>();
	[Header("Obstacles")]
	public ParagliderFlyingObjSpawnPoint monileObjectsSpawnPoints;
	public mobileObstacle[] flyingObstacles = new mobileObstacle[12] ;
	public staticObstacle[] staticObstacles = new staticObstacle[12] ;
	public List<ParagliderFlyingObstacle> availableMobilObstacles;



	[System.Serializable]
	public class mobileObstacle
	{
		public GameObject Object;
		 
		public int maxAppearances=0;
		public float maxSpeed=0;
		public bool canAppear=false;

		public mobileObstacle ()
		{
			maxAppearances=0;
			maxSpeed=0;
			canAppear=false;
//			GameObject = null;a
		}

	}

	[CustomPropertyDrawer(typeof(mobileObstacle))]
	public class ObstacleDrawer : PropertyDrawer 
	{
	    // Draw the property inside the given rect
	    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
	    {
	        // Using BeginProperty / EndProperty on the parent property means that
	        // prefab override logic works on the entire property.
	        EditorGUI.BeginProperty(position, label, property);
	        // Draw label
	        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
	        // Don't make child fields be indented
	        int indent = EditorGUI.indentLevel;
	        EditorGUI.indentLevel = 0;
	        // draw
			position.x = 100;
			position.x += labeledField(position.x , position.y,0,Mathf.Max(50,200),position.height,"",property.FindPropertyRelative ("Object"));
			position.x += labeledField(position.x, position.y,40,25,position.height,"count",property.FindPropertyRelative ("maxAppearances"));
			if (property.FindPropertyRelative("maxAppearances").intValue>0)
			{
				position.x += labeledField(position.x, position.y,40,35,position.height,"v max",property.FindPropertyRelative ("maxSpeed"));
				position.x += labeledField(position.x, position.y,30,5,position.height,"fade",property.FindPropertyRelative ("canAppear"));
			}
	        // Set indent back to what it was
	        EditorGUI.indentLevel = indent;
	        EditorGUI.EndProperty();
	    }
	}

	[System.Serializable]
	public class staticObstacle
	{
		public ParagliderStaticObstackles Object;
		public int maxAppearances=0;

		public void distribute()
		{
			if (Object!=null)
			maxAppearances = Object.distribute(maxAppearances);
		}

	}


	[CustomPropertyDrawer(typeof(staticObstacle))]
	public class staticObstacleDrawer : PropertyDrawer 
	{
	    // Draw the property inside the given rect
	    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
	    {
	        // Using BeginProperty / EndProperty on the parent property means that
	        // prefab override logic works on the entire property.
	        EditorGUI.BeginProperty(position, label, property);
	        // Draw label
	        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
	        // Don't make child fields be indented
	        int indent = EditorGUI.indentLevel;
	        EditorGUI.indentLevel = 0;
	        // draw
			position.x = 100;
			position.x += labeledField(position.x , position.y,0,Mathf.Max(50,200),position.height,"",property.FindPropertyRelative ("Object"));
			position.x += labeledField(position.x, position.y,40,25,position.height,"count",property.FindPropertyRelative ("maxAppearances"));
	        // Set indent back to what it was
	        EditorGUI.indentLevel = indent;
	        EditorGUI.EndProperty();
	    }

	}

	public static float labeledField(float x,float y,float wLabel, float wContent, float h, string label, SerializedProperty prop)
	    {
				
				EditorGUI.LabelField(new Rect(x, y, wLabel, h),label);
				EditorGUI.PropertyField(new Rect(x+wLabel,y,wContent,h), prop,GUIContent.none, true);
				return wLabel+wContent;
	    }


	public void orderMobileObstacles(mobileObstacle[] Array)
	{
		availableMobilObstacles.Clear();
		for(int i = 0;i<Array.Length;i++)
		{
			if (Array[i].Object!=null && Array[i].maxAppearances>0)
			{
				for (int j=0;j<Array[i].maxAppearances;j++)
				{
					availableMobilObstacles.Add(Array[i].Object.GetComponent<ParagliderFlyingObstacle>());
                    //Debug.Log("█ available onject added:" + availableMobilObstacles[availableMobilObstacles.Count-1]);
                }
			}
		}
	}

	public ParagliderFlyingObstacle spawnFlyingObject()
	{
        if (availableMobilObstacles.Count > 0)
        {
            int i = UnityEngine.Random.Range(0, availableMobilObstacles.Count);
            Debug.Log("level "+SceneManager.GetActiveScene().name+" - fetching index " + i + " out of " + availableMobilObstacles.Count + "available objetcs");
            GameObject copy = Instantiate(availableMobilObstacles[i].gameObject);
            copy.transform.parent = terrain.transform;
            copy.SetActive(true);
            ParagliderFlyingObstacle output = copy.GetComponent<ParagliderFlyingObstacle>();
            availableMobilObstacles.RemoveAt(i);
            Debug.Log(i + "of" + availableMobilObstacles.Count + " available objects is a " + copy.name + " with script " + output + "is ready to be spawned");
            output.spawn();
            return output;
        }
        return null;
	}



	static public GameObject[] cleanUpArray(GameObject[] Array)
	{
		
		int j=0;
		for(int i = 0;i<Array.Length;i++)
		{
			if (Array[i]!=null)
			{
				if(j<i)
				{
					Array[j]=Array[i];
					Array[i]=null;
				}
				j++;
			}
		}
		GameObject[] Output = new GameObject[j];
		for(int i = 0;i<Output.Length;i++)
		{
			Output[i]=Array[i];
		}
		return Output;
	}

	public void distributeStatObst()
	{
		foreach( staticObstacle obst in staticObstacles)
		{
			obst.distribute();
		}
	}


	// Use this for initialization
	void Start () {
		Debug.Log(">>>>>>>>>>> setting up Level ("+gameObject.name+") <<<<<<<<<<<<<<<");
		distributeStatObst();
		orderMobileObstacles(flyingObstacles);
	}
	// Update is called once per frame
	void Update () {

	}
}
