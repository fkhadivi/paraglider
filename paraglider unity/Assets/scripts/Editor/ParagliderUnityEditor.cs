using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

[CustomPropertyDrawer(typeof(ParagliderLevel.mobileObstacle))]
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
        position.x += paragliderUnityEditior.labeledField(position.x, position.y, 0, Mathf.Max(50, 200), position.height, "", property.FindPropertyRelative("Object"));
        position.x += paragliderUnityEditior.labeledField(position.x, position.y, 40, 25, position.height, "count", property.FindPropertyRelative("maxAppearances"));
        if (property.FindPropertyRelative("maxAppearances").intValue > 0)
        {
            position.x += paragliderUnityEditior.labeledField(position.x, position.y, 40, 35, position.height, "v max", property.FindPropertyRelative("maxSpeed"));
            position.x += paragliderUnityEditior.labeledField(position.x, position.y, 30, 5, position.height, "fade", property.FindPropertyRelative("canAppear"));
        }
        // Set indent back to what it was
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}




[CustomPropertyDrawer(typeof(ParagliderLevel.staticObstacle))]
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
        position.x += paragliderUnityEditior.labeledField(position.x, position.y, 0, Mathf.Max(50, 200), position.height, "", property.FindPropertyRelative("Object"));
        position.x += paragliderUnityEditior.labeledField(position.x, position.y, 40, 25, position.height, "count", property.FindPropertyRelative("maxAppearances"));
        // Set indent back to what it was
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

}

public class paragliderUnityEditior : MonoBehaviour
{

    public static float labeledField(float x, float y, float wLabel, float wContent, float h, string label, SerializedProperty prop)
    {

        EditorGUI.LabelField(new Rect(x, y, wLabel, h), label);
        EditorGUI.PropertyField(new Rect(x + wLabel, y, wContent, h), prop, GUIContent.none, true);
        return wLabel + wContent;
    }
}


