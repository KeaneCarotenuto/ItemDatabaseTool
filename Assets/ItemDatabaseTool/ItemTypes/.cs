using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

//[REQUIRED]
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// An example of a custom item type that can be used in the item database.
/// </summary>
//Make sure to set the [Serializable] attribute on this class, so that it can be saved!
[Serializable]
public class  : Item
{
    // make sure to set variables as serialized fields in the inspector, so that they can be saved!
    [SerializeField] public float m_exampleValue = 0;

    /// <summary>
    /// [REQUIRED] Used to create a copy of the item. Make sure to set any unique values here!
    /// </summary>
    public override Item CreateInstance()
    {
        // [REQUIRED] Create base item
         newItem = ()base.CreateInstance();

        // Setting unique values here:
        // example valye
        newItem.m_exampleValue = m_exampleValue;

        return newItem;
    }

    
    //Custom editor for this class
    #if UNITY_EDITOR
    [CustomEditor(typeof())]
    public class Editor : ItemEditor
    {
        public override void OnInspectorGUI()
        {
            // [REQUIRED] draw base editor (Item in this case)
            base.OnInspectorGUI();
            // [REQUIRED] get the editor target
             item = ()target;

            // red box for weapon stats
            GUI.backgroundColor = Color.red;
            GUILayout.BeginVertical("box");
            GUI.backgroundColor = Color.white;
            // bold center text
            GUILayout.Label(" Stats", CustomEditorStuff.center_bold_label);

            // Your custom values here
            item.m_exampleValue = EditorGUILayout.FloatField("Example Value: ", item.m_exampleValue);

            //end red box
            GUILayout.EndVertical();
        }
    }
    #endif
	
	
}