using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class CustomEditorStuff
{
    public static GUIStyle center_bold_label = new GUIStyle(EditorStyles.label) {
        alignment = TextAnchor.MiddleCenter,
        fontStyle = FontStyle.Bold
    };

    public static GUIStyle center_label = new GUIStyle(EditorStyles.label) {
        alignment = TextAnchor.MiddleCenter
    };

    public static GUIStyle bold_label = new GUIStyle(EditorStyles.label) {
        fontStyle = FontStyle.Bold
    };
}

public class ReadOnlyAttribute : PropertyAttribute
{
}
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
     public override float GetPropertyHeight(SerializedProperty property,
                                             GUIContent label)
     {
         return EditorGUI.GetPropertyHeight(property, label, true);
     }
     public override void OnGUI(Rect position,
                                SerializedProperty property,
                                GUIContent label)
     {
         GUI.enabled = false;
         EditorGUI.PropertyField(position, property, label, true);
         GUI.enabled = true;
     }
}
#endif
