using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text.RegularExpressions;
// json serialization

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

    [SerializeField] private string m_id = "";
    [SerializeField] public string id
    {
        get { return m_id; }
        set
        {
            m_id = value;
            ValidateID();
        }
    }

    [SerializeField] private string m_variantID = "";
    [SerializeField] public string variantID
    {
        get { return m_variantID; }
        set
        {
            m_variantID = value;
        }
    }
    

    [SerializeField] public string m_displayName = "";
    [SerializeField] public string m_description = "";
    [SerializeField] public Sprite m_icon = null;

    [SerializeField] public List<TagManager.Tag> m_tags = new List<TagManager.Tag>();

    public void ValidateID(){
        //correct id
        //to lower
        m_id = m_id.ToLower();
        //replace non letters and numbers with ""
        m_id = Regex.Replace(m_id, @"[^a-zA-Z0-9_]", "");
        //replace space with _
        m_id = m_id.Replace(" ", "_");
    }

    /// <summary>
    /// Make variant of this item.
    /// </summary>
    /// <returns>The variant.</returns>
    public virtual Item CreateVariant(string _type)
    {
        Item item = (Item)Item.CreateInstance(_type);
        item.variantID = Guid.NewGuid().ToString();
        item.m_id = this.id + "_" + item.variantID;
        item.m_displayName = this.m_displayName;
        item.m_description = this.m_description;
        item.m_icon = this.m_icon;
        item.m_tags = this.m_tags;
        return item;
    }

    /// <summary>
    /// Saves item to file
    /// </summary>
    /// <param name="_path">path to save to</param>
    public static void Save(string _path, Item _item)
    {
        FileStream file = File.Create(_path);

        //serialize item
        string json = JsonUtility.ToJson(_item);
        string itemType = _item.GetType().Name;

        //make writer
        StreamWriter writer = new StreamWriter(file);
        // write the item type
        writer.Write(itemType);
        // new line
        writer.Write("\n");
        // write to file
        writer.Write(json);
        writer.Close();

        file.Close();
    }

    /// <summary>
    /// Loads item from file, can return any item type
    /// </summary>
    /// <param name="path">path to load from</param>
    /// <returns>Item loaded from file</returns>
    public static Item Load(string path)
    {
        //read file
        FileStream file = File.Open(path, FileMode.Open);
        StreamReader reader = new StreamReader(file);
        string itemType = reader.ReadLine();
        string json = reader.ReadToEnd();
        reader.Close();
        file.Close();

        //deserialize item as type
        ScriptableObject item = ScriptableObject.CreateInstance(itemType);

        // cast to item type
        JsonUtility.FromJsonOverwrite(json, item);

        // cast to item type
        return item as Item;
    }


    #if UNITY_EDITOR
    //inspector gui stuff
    [CustomEditor(typeof(Item))]
    public class ItemEditor : Editor
    {
        static bool showTags = true;

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

            // horiz
            GUILayout.BeginHorizontal();
            item.id = EditorGUILayout.TextField("ID: ", item.id);
            // update file name button
            if (GUILayout.Button("Update File Name"))
            {
                item.ValidateID();

                //update file name
                string path = AssetDatabase.GetAssetPath(item);
                string newPath = path.Replace(item.id, item.id);
                AssetDatabase.RenameAsset(path, item.id);
            }
            GUILayout.EndHorizontal();
            item.m_displayName = EditorGUILayout.TextField("Display Name: ", item.m_displayName);
            item.m_description = EditorGUILayout.TextField("Description: ", item.m_description);
            item.m_icon = EditorGUILayout.ObjectField("Icon: ", item.m_icon, typeof(Sprite), false) as Sprite;

            //tags
            showTags = EditorGUILayout.Foldout(showTags, "Tags", true, new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold});
            if (showTags){
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

                    // save
                    EditorUtility.SetDirty(item);
                }
                GUILayout.EndVertical();
            }

            //end green box
            GUILayout.EndVertical();


            // on change save
            if (GUI.changed)
            {
                EditorUtility.SetDirty(item);
            }
        }
    }
    #endif
}