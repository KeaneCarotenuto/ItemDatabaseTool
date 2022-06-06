using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;

public class DatabaseWindow : EditorWindow {

    static string itemPath = "Assets/ItemDatabaseTool/Resources/Items/";

    static string searchText = "";

    public static Item selected;
    static public int selectedType;

    Vector2 listScroll = Vector2.zero;
    Vector2 itemScroll = Vector2.zero;

    // Open window button
    [MenuItem("ItemDatabaseTool/Database Window")]
    private static void ShowWindow() {
        EditorWindow window = GetWindow<DatabaseWindow>();
        window.titleContent = new GUIContent("Database Window");
        window.Show();
    }

    // Remake the database .json files
    [MenuItem("ItemDatabaseTool/Re-make Database folder")]
    private static void ResetDatabaseFolder() {
        ItemDatabase.ResetDatabaseFolder();
    }

    private void OnValidate() {
        ItemDatabase.Refresh();
    }

    private void OnEnable() {
        ItemDatabase.Refresh();
    }

    private void OnGUI()
    {
        //check selected
        if (!selected)
        {
            selected = ItemDatabase.database.FirstOrDefault();
        }
        

        GUILayout.BeginHorizontal();
        DrawList();
        DrawItemInspector();
        GUILayout.EndHorizontal();

        //Refresh();
        
        //save on change
        if (GUI.changed)
        {
            ItemDatabase.Refresh();
            EditorUtility.SetDirty(this);
        }

        Event e = Event.current;
        Rect windowRect = new Rect(0, 0, position.width, position.height);
        if (GUI.Button(windowRect, "", GUIStyle.none))
        {
            GUI.FocusControl(null);
        }
    }

    private void DrawList()
    {
        //draw scroll list of items
        listScroll = GUILayout.BeginScrollView(listScroll, false, true, GUILayout.Width(position.width / 2.5f), GUILayout.Height(position.height));
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Items", CustomEditorStuff.center_bold_label);
        // save to file button
        if (GUILayout.Button(new GUIContent("Re-make Files", "DELETES ALL .json and .meta files in the ItemDatabase folder.\n\nThen re-makes the current database into .json files"), GUILayout.Width(100)))
        {
            ItemDatabase.ResetDatabaseFolder();
        }
        //refresh button
        if (GUILayout.Button(new GUIContent("Refresh", "Refreshes the list below"), GUILayout.MaxWidth(75)))
        {
            ItemDatabase.Refresh();
        }
        GUILayout.EndHorizontal();

        //space
        GUILayout.Space(10);

        // create new item
        GUILayout.BeginHorizontal();
        string[] names = Item.AllTypes.Select(t => t.Name).ToArray();
        names = names.Concat(new string[] { typeof(Item).Name }).ToArray();

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button(new GUIContent("Create New", "Create new item of selected type")))
        {
            CreateNewItem(names[selectedType]);
        }

        selectedType = EditorGUILayout.Popup(selectedType,names);
        GUI.backgroundColor = Color.white;
        GUILayout.EndHorizontal();

        // choose path
        GUILayout.BeginHorizontal();
        GUILayout.Label("Path: " + EditorPrefs.GetString("ItemDatabaseTool_Path", itemPath));
        if (GUILayout.Button(new GUIContent("Change", "Change the path to the items folder")))
        {
            string path = EditorUtility.OpenFolderPanel("Choose Items Folder", "", "");

            // check if path is valid
            if (path.Length != 0)
            {
                // path must be in this project
                if (path.Contains(Application.dataPath))
                {
                    itemPath = path.Replace(Application.dataPath, "Assets");

                    // save
                    EditorPrefs.SetString("ItemDatabaseTool_ItemPath", itemPath);
                }
                else
                {
                    Debug.LogError("Path must be within assets folder");
                }
            }
        }
        GUILayout.EndHorizontal();

        //space
        GUILayout.Space(10);

        //search bar
        GUILayout.BeginHorizontal();
        GUILayout.Label("Search: ", GUILayout.MaxWidth(50));
        searchText = GUILayout.TextField(searchText, GUILayout.MinWidth(100));
        if (GUILayout.Button(new GUIContent("Clear", "Clears the search bar"), GUILayout.MaxWidth(75)))
        {
            searchText = "";
        }
        GUILayout.EndHorizontal();

        for (int i = 0; i < ItemDatabase.database.Count; i++)
        {
            //if id or display name doesnt match search text, skip
            if (!ItemDatabase.database[i].id.ToLower().Contains(searchText.ToLower()) && !ItemDatabase.database[i].m_displayName.ToLower().Contains(searchText.ToLower()))
            {
                continue;
            }

            //horiz
            GUILayout.BeginHorizontal();
            //begind disabled group
            EditorGUI.BeginDisabledGroup(ItemDatabase.database[i] == selected);
            if (GUILayout.Button(ItemDatabase.database[i].id))
            {
                selected = ItemDatabase.database[i];
                GUI.FocusControl(null);
            }
            EditorGUI.EndDisabledGroup();
            
            GUI.backgroundColor = Color.red;
            //delete button
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                GUI.backgroundColor = Color.white;
                //popup to confirm
                if (EditorUtility.DisplayDialog("Delete Item", "Are you sure you want to delete " + ItemDatabase.database[i].name + "?\nThis CANNOT be undone!", "DELETE", "KEEP"))
                {
                     //delete from asset database
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(ItemDatabase.database[i]));
                    //remove from list
                    ItemDatabase.database.RemoveAt(i);
                    i--;
                    //refresh
                    ItemDatabase.Refresh();
                }
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
        }

        //space
        GUILayout.Space(10);

        GUILayout.EndScrollView();
    }

    private void DrawItemInspector()
    {
        //vert
        GUILayout.BeginVertical();
        itemScroll = GUILayout.BeginScrollView(itemScroll);
        //horiz
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Selected: ");
        //disable group
        EditorGUI.BeginDisabledGroup(true);
        selected = EditorGUILayout.ObjectField(selected, typeof(Item), false) as Item;
        EditorGUI.EndDisabledGroup();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        //if selected item is not null, show the selected item
        if (selected != null)
        {
            Editor editor = Editor.CreateEditor(selected);
            editor?.OnInspectorGUI();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    /// <summary>
    /// Creates a new scriptable object item in unity
    static public void CreateNewItem(string type)
    {
        // if path doesnt exist, create it
        if (!Directory.Exists(itemPath))
        {
            Directory.CreateDirectory(itemPath);
        }

        string assetType = type;

        //create the item
        ScriptableObject item = ScriptableObject.CreateInstance(assetType);

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(itemPath + "/" + assetType + ".asset");

        //save
        AssetDatabase.CreateAsset(item, assetPathAndName);
        AssetDatabase.SaveAssets();

        //refresh
        AssetDatabase.Refresh();

        //cast item to item
        Item newItem = item as Item;
        //if item is not null, select it
        if (newItem != null)
        {
            //select it
            selected = newItem;
        }

        ItemDatabase.Refresh();
    }
}

#endif