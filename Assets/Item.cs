using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

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
        id = this.GetType().Name + "_0";
        m_displayName = "New Item";
        m_description = "New Item Description";
        m_icon = null;
    }

    private string m_id = "";
    public string id
    {
        get { return id; }
        set
        {
            id = value;
            //ValidateID();
        }
    }
    

    public string m_displayName = "";
    public string m_description = "";
    public Sprite m_icon = null;

    public List<TagManager.Tag> m_tags = new List<TagManager.Tag>();

    public void ValidateID(){
        // //correct id
        // //to lower
        // m_id = m_id.ToLower();
        // //replace non letters and numbers with ""
        // m_id = Regex.Replace(m_id, @"[^a-zA-Z0-9_]", "");
        // //replace space with _
        // m_id = m_id.Replace(" ", "_");
    }


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
            item.m_displayName = EditorGUILayout.TextField("Display Name: ", item.m_displayName);
            item.m_description = EditorGUILayout.TextField("Description: ", item.m_description);
            item.m_icon = EditorGUILayout.ObjectField("Icon: ", item.m_icon, typeof(Sprite), false) as Sprite;

            //tags
            GUILayout.Label("Tags", CustomEditorStyles.center_bold_label);
            GUI.backgroundColor = Color.white;
            GUILayout.BeginVertical("box");
            //draw tags
            for (int i = 0; i < item.m_tags.Count; i++)
            {
                GUILayout.BeginHorizontal();
                item.m_tags[i].name = EditorGUILayout.TextField("Tag Name: ", item.m_tags[i].name);
                item.m_tags[i].payload = EditorGUILayout.TextField("Payload: ", item.m_tags[i].payload);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    item.m_tags.RemoveAt(i);
                }
                GUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add Tag"))
            {
                item.m_tags.Add(new TagManager.Tag("New Tag"));
            }
            GUILayout.EndVertical();

            //end green box
            GUILayout.EndVertical();
        }
    }
    #endif
}