using UnityEngine;
using UnityEditor;

public class SelectDuplicateReplace : EditorWindow
{
    [SerializeField] private GameObject prefab;

	[MenuItem("Tools/Select Duplicate Replace")]
    static void CreateSelectDuplicateReplace()
    {
        EditorWindow.GetWindow<SelectDuplicateReplace>();
    }
	bool dublicate = false;
	Vector3 offset;
    private void OnGUI()
	{	
		bool mat = EditorGUILayout.Toggle("Same Material", false);
		bool mesh = EditorGUILayout.Toggle("Same Mesh", false);
		bool Pos = EditorGUILayout.Toggle("Same position", false);
		bool scale = EditorGUILayout.Toggle("Same scale", false);
		if (GUILayout.Button("select")) doSelect( mat, mesh, Pos, scale);

		offset = EditorGUILayout.Vector3Field("Offset",offset);
		dublicate = EditorGUILayout.Toggle("dublicate", dublicate);
		if (GUILayout.Button("Move")) doMove();

																	
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
		if (GUILayout.Button("Replace")) doReplace();
        GUI.enabled = false;
        EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
		
    }

	private void doSelect(bool mat,bool mesh,bool Pos,bool scale)
    {

    }

	private void doMove()
	{
		var selection = Selection.gameObjects;

        for (var i = selection.Length - 1; i >= 0; --i)
        {
            var selected = selection[i];
            if(dublicate)
            {
                GameObject newObject = Instantiate(selected);// PrefabUtility.InstantiatePrefab(PrefabUtility.GetCorrespondingObjectFromSource(selected));
				if(newObject == null) newObject = Instantiate(selected);
				newObject.name = selected.name;
				Undo.RegisterCreatedObjectUndo(newObject, "Dublicate");
				newObject.transform.parent = selected.transform.parent;
            	newObject.transform.localPosition = selected.transform.localPosition;
            	newObject.transform.localRotation = selected.transform.localRotation;
            	newObject.transform.localScale = selected.transform.localScale;
            	newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());
            }
			selected.transform.position = selected.transform.position+offset;
			Debug.Log(selected.name+"moved");
        } 
	}


    private void doReplace()
    {
	var selection = Selection.gameObjects;

        for (var i = selection.Length - 1; i >= 0; --i)
        {
            var selected = selection[i];
            var prefabType = PrefabUtility.GetPrefabType(prefab);
            GameObject newObject;

            if (prefabType == PrefabType.Prefab)
            {
                newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            }
            else
            {
                newObject = Instantiate(prefab);
                newObject.name = prefab.name;
            }

            if (newObject == null)
            {
                Debug.LogError("Error instantiating prefab");
                break;
            }

            Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
            newObject.transform.parent = selected.transform.parent;
            newObject.transform.localPosition = selected.transform.localPosition;
            newObject.transform.localRotation = selected.transform.localRotation;
            newObject.transform.localScale = selected.transform.localScale;
            newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());
            Undo.DestroyObjectImmediate(selected);
        }

    }
}