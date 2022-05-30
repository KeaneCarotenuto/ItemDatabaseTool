using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;

public class DatabaseWindow : EditorWindow {

    public static List<Item> database = new List<Item>();
    static Item temp;

    static string searchText = "";

    static Item selected;

    public int selectedType;

    Vector2 listScroll = Vector2.zero;
    Vector2 itemScroll = Vector2.zero;

    [MenuItem("ItemDatabaseTool/DatabaseWindow")]
    private static void ShowWindow() {
        EditorWindow window = GetWindow<DatabaseWindow>();
        window.titleContent = new GUIContent("DatabaseWindow");
        window.Show();
    }

    private void OnValidate() {
        Refresh();
    }

    private void OnEnable() {
        Refresh();
    }

    private void OnGUI()
    {
        //check selected
        if (!selected)
        {
            selected = database.FirstOrDefault();
        }
        //check database
        for (int i = 0; i < database.Count; i++)
        {
            if (!database[i])
            {
                database.RemoveAt(i);
                i--;
            }
        }

        GUILayout.BeginHorizontal();
        DrawList();
        DrawItemInspector();
        GUILayout.EndHorizontal();

        //Refresh();

        //save on change
        if (GUI.changed)
        {
            EditorUtility.SetDirty(this);
            Refresh();
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
        GUILayout.Label("Items", CustomEditorStyles.center_bold_label);
        //refresh button
        if (GUILayout.Button("Refresh", GUILayout.MaxWidth(75)))
        {
            Refresh();
        }
        GUILayout.EndHorizontal();

        //space
        GUILayout.Space(10);

        //search bar
        GUILayout.BeginHorizontal();
        GUILayout.Label("Search: ", GUILayout.MaxWidth(50));
        searchText = GUILayout.TextField(searchText, GUILayout.MinWidth(100));
        if (GUILayout.Button("Clear", GUILayout.MaxWidth(75)))
        {
            searchText = "";
        }
        GUILayout.EndHorizontal();

        for (int i = 0; i < database.Count; i++)
        {
            //if id or display name doesnt match search text, skip
            if (!database[i].id.ToLower().Contains(searchText.ToLower()) && !database[i].displayName.ToLower().Contains(searchText.ToLower()))
            {
                continue;
            }

            //horiz
            GUILayout.BeginHorizontal();
            //begind disabled group
            EditorGUI.BeginDisabledGroup(database[i] == selected);
            if (GUILayout.Button(database[i].id))
            {
                selected = database[i];
                GUI.FocusControl(null);
            }
            EditorGUI.EndDisabledGroup();
            
            GUI.backgroundColor = Color.red;
            //delete button
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                GUI.backgroundColor = Color.white;
                //popup to confirm
                if (EditorUtility.DisplayDialog("Delete Item", "Are you sure you want to delete " + database[i].name + "?\nThis CANNOT be undone!", "DELETE", "KEEP"))
                {
                     //delete from asset database
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(database[i]));
                    //remove from list
                    database.RemoveAt(i);
                    i--;
                    //refresh
                    Refresh();
                }
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
        }

        //space
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        string[] names = Item.AllTypes.Select(t => t.Name).ToArray();
        names = names.Concat(new string[] { typeof(Item).Name }).ToArray();

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Create New"))
        {
            CreateNewItem(names[selectedType]);
        }

        selectedType = EditorGUILayout.Popup(selectedType,names);
        GUI.backgroundColor = Color.white;

        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();
    }

    private void DrawItemInspector()
    {
        //vert
        GUILayout.BeginVertical();
        itemScroll = GUILayout.BeginScrollView(itemScroll);
        //horiz
        GUILayout.BeginHorizontal();
        //label
        GUILayout.Label("Selected: ");
        //disable group
        EditorGUI.BeginDisabledGroup(true);
        selected = EditorGUILayout.ObjectField(selected, typeof(Item), false) as Item;
        EditorGUI.EndDisabledGroup();
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
        string assetType = type;

        string  path = "Assets";

        //create the item
        ScriptableObject item = ScriptableObject.CreateInstance(assetType);

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + assetType + ".asset");

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

        Refresh();
    }

    static public void Refresh() {
        ValidateIDs();

        database.Clear();
        var guids = AssetDatabase.FindAssets("t:Item");
        for (int i = 0; i < guids.Length; i++) {
            var path = AssetDatabase.GUIDToAssetPath(guids[i]);
            var asset = AssetDatabase.LoadAssetAtPath<Item>(path);
            database.Add(asset);
        }

        //sort by id
        database.Sort((a, b) => a.id.CompareTo(b.id));
    }

    static public void ValidateIDs() {
        //correct ID values
        for (int i = 0; i < database.Count; i++) {
            //to lower
            database[i].id = database[i].id.ToLower();
            //replace non letters and numbers with _
            database[i].id = Regex.Replace(database[i].id, @"[^a-zA-Z0-9_]", "_");
            //replace space with _
            database[i].id = database[i].id.Replace(" ", "_");
        }

        //correct duplicate IDs
        for (int i = 0; i < database.Count; i++) {
            for (int j = 0; j < database.Count; j++) {
                if (database[i].id == database[j].id && i != j) {
                    //check if last char is a number
                    if (char.IsNumber(database[i].id[database[i].id.Length - 1])) {
                        //go backwards and count numbers
                        int count = 0;
                        for (int k = database[i].id.Length - 1; k >= 0; k--) {
                            if (char.IsNumber(database[i].id[k])) {
                                count++;
                            } else {
                                break;
                            }
                        }
                        //add one to the number
                        int number = int.Parse(database[i].id.Substring(database[i].id.Length - count, count)) + 1;
                        //remove last numbers
                        database[i].id = database[i].id.Substring(0, database[i].id.Length - count);
                        //add new number
                        database[i].id += number;

                        ValidateIDs();
                    }
                    else {
                        //add _1 to the number
                        database[i].id = database[i].id + "_1";
                    }
                }
            }
        }
    }
}

#endif