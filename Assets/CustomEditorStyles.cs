using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class CustomEditorStyles
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
#endif
