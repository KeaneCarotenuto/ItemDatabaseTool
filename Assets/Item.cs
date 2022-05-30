using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
using System.Linq;

#endif
[Serializable]
public class Item : ScriptableObject
{
    public static readonly IEnumerable<System.Type> AllTypes;

    static Item()
    {
        System.Type type = typeof(Item);
        AllTypes = type.Assembly.GetTypes().Where(t => t.IsSubclassOf(type));
    }

    //simple constructor
    public Item()
    {
        //set default values
        id = typeof(Item).Name + "_0";
        displayName = "New Item";
        description = "New Item Description";
        icon = null;
    }

    public string id = "";
    public string displayName = "";
    public string description = "";
    public Sprite icon = null;


    #if UNITY_EDITOR
    //inspector gui stuff
    [CustomEditor(typeof(Item))]
    public class ItemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawUI();
        }

        public void DrawUI()
        {
            Item item = (Item)target;

            GUI.backgroundColor = Color.green;
            //green box for item
            GUILayout.BeginVertical("box");
            // bold text
            GUILayout.Label("Base Item Stats", CustomEditorStyles.center_bold_label);
            GUI.backgroundColor = Color.white;

            item.id = EditorGUILayout.TextField("ID: ", item.id);
            item.displayName = EditorGUILayout.TextField("Display Name: ", item.displayName);
            item.description = EditorGUILayout.TextField("Description: ", item.description);
            item.icon = EditorGUILayout.ObjectField("Icon: ", item.icon, typeof(Sprite), false) as Sprite;

            //end green box
            GUILayout.EndVertical();
        }
    }
    #endif
}