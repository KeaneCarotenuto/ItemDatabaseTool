using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[Serializable]
public class ItemDatabase
{
    // constructor
    static ItemDatabase(){
        #if UNITY_EDITOR
        Refresh();
        Initialize();
        #endif
    }

    [SerializeField] public static List<Item> database = new List<Item>();

    /// <summary>
    /// Gets an item by its id.
    /// </summary>
    /// <param name="id">The id of the item.</param>
    /// <returns>The item with the given id.</returns>
    public static Item GetItem(string id)
    {
        Item item = database.Find(x => x.id == id);
        if (item == null)
        {
            Debug.LogWarning("Item with id " + id + " not found.");
        }
        return item;
    }

    /// <summary>
    /// get list of items in the database by id
    /// </summary>
    public static List<Item> GetItemsById(string _id)
    {
        return database.Where(x => x.id.ToLower().Contains(_id.ToLower())).ToList();
    }

    /// <summary>
    /// get list of items in the database by name
    /// </summary>
    public static List<Item> GetItemsByName(string _name)
    {
        return database.Where(x => x.m_displayName.ToLower().Contains(_name.ToLower())).ToList();
    }

    /// <summary>
    /// get list of items in the database by description
    /// </summary>
    public static List<Item> GetItemsByDescription(string _description)
    {
        return database.Where(x => x.m_description.ToLower().Contains(_description.ToLower())).ToList();
    }

    /// <summary>
    /// get list of items in the database by type (System.Type)
    /// </summary>
    public static List<Item> GetItemsByType(System.Type _type)
    {
        return database.Where(x => x.GetType() == _type).ToList();
    }

    /// <summary>
    /// get list of items in the database by type (string) <br/>
    /// NOTE: Case sensitive!
    /// </summary>
    public static List<Item> GetItemsByType(string _type)
    {
        return database.Where(x => x.GetType().Name == _type).ToList();
    }

    /// <summary>
    /// get list of items in the database using optional search parameters using OR logic
    /// </summary>
    public static List<Item> GetItemsByORFilter(string _id = "", string _name = "", string _description = "", string _type_s = "", System.Type _type_t = null)
    {
        List<Item> items = new List<Item>();

        bool anyFilter = false;
        if (_id != "")
        {
            anyFilter = true;

            items = items.Union(GetItemsById(_id)).ToList();
        }

        if (_name != "")
        {
            anyFilter = true;

            items = items.Union(GetItemsByName(_name)).ToList();
        }

        if (_description != "")
        {
            anyFilter = true;

            items = items.Union(GetItemsByDescription(_description)).ToList();
        }

        if (_type_s != "")
        {
            anyFilter = true;

            items = items.Union(GetItemsByType(_type_s)).ToList();
        }

        if (_type_t != null)
        {
            anyFilter = true;

            items = items.Union(GetItemsByType(_type_t)).ToList();
        }

        // if no filters were used, return all items
        if (!anyFilter)
        {
            return database;
        }

        return items;
    }

    /// <summary>
    /// get list of items in the database using optional search parameters using AND logic
    /// </summary>
    public static List<Item> GetItemsByANDFilter(string _id = "", string _name = "", string _description = "" , string _type_s = "", System.Type _type_t = null)
    {
        List<Item> items = new List<Item>();

        bool anyFilter = false;
        if (_id != "")
        {
            anyFilter = true;

            items = items.Intersect(GetItemsById(_id)).ToList();
        }

        if (_name != "")
        {
            anyFilter = true;

            items = items.Intersect(GetItemsByName(_name)).ToList();
        }

        if (_description != "")
        {
            anyFilter = true;

            items = items.Intersect(GetItemsByDescription(_description)).ToList();
        }

        if (_type_s != "")
        {
            anyFilter = true;

            items = items.Intersect(GetItemsByType(_type_s)).ToList();
        }

        if (_type_t != null)
        {
            anyFilter = true;

            items = items.Intersect(GetItemsByType(_type_t)).ToList();
        }

        // if no filters were used, return all items (redundant for this function, but included for consistency ??\_(???)_/?? )
        if (!anyFilter)
        {
            return database;
        }

        return items;
    }


    public static string GetDatabaseFoldersPath()
    {
        return "/ItemDatabase/database/";
    }

    public static string GetSavePath(){
        return Application.dataPath + GetDatabaseFoldersPath();
    }

    [RuntimeInitializeOnLoadMethod]
    public static void Initialize()
    {
        if (database.Count == 0)
        {
            LoadListFromFile();
        }

        
        CleanUpFiles();
    }


    #if UNITY_EDITOR
    /// <summary>
    /// On Build, refresh database, saves to file, Copies files from editor folder to build folder <br/>
    /// NOTE: works on windows/linux/mac build (I THINK!)
    /// </summary>
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
        Debug.Log( "Made build at: " + pathToBuiltProject );

        // refresh db
        Refresh();
        // save db to file
        SaveListToFile();

        // make databse folder in build folder
        string buildFolder = pathToBuiltProject.Substring(0, pathToBuiltProject.LastIndexOf("/")) ;
        string productName = Application.productName;
        string buildDatabasePath = buildFolder + "/" + productName + "_Data" + GetDatabaseFoldersPath();
        Debug.Log("Creating database at: " + buildDatabasePath);
        if (!Directory.Exists(buildDatabasePath))
        {
            Directory.CreateDirectory(buildDatabasePath);
            Debug.Log("Created database folder");
        }

        // get all files in the ItemDatabase folder
        string[] files = Directory.GetFiles(GetSavePath(), "*.json");
        foreach (string file in files)
        {
            // get the file name
            string fileName = Path.GetFileName(file);
            // copy the file to the build folder
            File.Copy(GetSavePath() + fileName, buildDatabasePath + fileName, true);

            Debug.Log("Copied file: " + fileName);
        }

        Debug.Log("Finished creating database");
    }
    #endif

    /// <summary>
    /// Saves the database to file from memory.
    /// </summary>
    static public void SaveListToFile()
    {
        string path = GetSavePath();
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        foreach (Item item in database)
        {
            string fileName = item.id + ".json";
            Item.Save(GetSavePath(), fileName, item);
        }
    }

    /// <summary>
    /// Loads the database from file into memory
    /// </summary>
    static public void LoadListFromFile()
    {
        string path = GetSavePath();
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        database.Clear();
        foreach (string filePath in Directory.GetFiles(path))
        {
            string fileName = Path.GetFileName(filePath);
            // if file does not ends in .json, continue
            if (!fileName.EndsWith(".json")) continue;
            
            Item item = Item.Load(GetSavePath(), fileName);
            database.Add(item);
        }
    }

    /// <summary>
    /// Deletes all .json files in the database folder
    /// </summary>
    static public void DeleteListFromFile()
    {
        Debug.Log("Deleting database");

        string path = GetSavePath();
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        foreach (string filePath in Directory.GetFiles(path))
        {
            string fileName = Path.GetFileName(filePath);
            // if file does not ends in .json, continue
            if (!fileName.EndsWith(".json")) continue;

            File.Delete(filePath);
        }
    }

    /// <summary>
    /// Deletes meta data files in the database folder
    /// </summary>
    static public void DeleteListMetaDataFromFile()
    {
        Debug.Log("Deleting database meta data");

        string path = GetSavePath();
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        foreach (string filePath in Directory.GetFiles(path))
        {
            string fileName = Path.GetFileName(filePath);
            // if file does not ends in .meta, continue
            if (!fileName.EndsWith(".meta")) continue;

            File.Delete(filePath);
        }
    }

    static public void CleanUpFiles()
    {
        Debug.Log("Cleaning up files");

        // if directory does not exist, create it (instance save folder)
        if (!Directory.Exists(Item.GetInstanceSavePath()))
        {
            Directory.CreateDirectory(Item.GetInstanceSavePath());
        }
        //if directory does not exist, create it (inventory save folder)
        if (!Directory.Exists(Inventory.GetInventoryFolder()))
        {
            Directory.CreateDirectory(Inventory.GetInventoryFolder());
        }

        // delete all instance files that are not in any inventories and not in any save files

        // get all files in the instance folder
        string[] instanceSaveFiles = Directory.GetFiles(Item.GetInstanceSavePath(), "*.json");
        // convert to file names


        // get all inventories in the game
        List<Inventory> inventories = Inventory.allInventories;

        // get all inventory save files by inventory name
        List<string> savedInstances = new List<string>();
        foreach (Inventory inventory in inventories)
        {
            //if file does not exist, continue
            if (!File.Exists(inventory.GetSaveFilePath())) continue;

            // get every line of the inventory save file
            List<string> lines = File.ReadAllLines(inventory.GetSaveFilePath()).ToList();

            // combine lines into savedInstances
            savedInstances = savedInstances.Union(lines).ToList();
        }

        // get all directories in the inventory folder
        string[] directories = Directory.GetDirectories(Inventory.GetInventoryFolder());

        foreach (string directory in directories)
        {
            List<string> files = Directory.GetFiles(directory, "inventory.json").ToList();

            foreach (string file in files)
            {
                // get every line of the inventory save file
                List<string> lines = File.ReadAllLines(file).ToList();

                savedInstances = savedInstances.Union(lines).ToList();
            }
        }

        // for each file in the instance folder, check if it is in the inventory or save files
        foreach (string filePath in instanceSaveFiles)
        {
            string fileName = Path.GetFileName(filePath);
            // if file does not ends in .json, continue
            if (!fileName.EndsWith(".json")) continue;

            // if file is in the inventory save files, continue
            if (savedInstances.Contains(fileName)) continue;

            bool inInventory = false;
            // if id and instance id exist in any inventory, continue
            foreach (Inventory inventory in inventories)
            {
                for (int i = 0; i < inventory.slots.Count; i++)
                {
                    Item item = inventory.slots[i].item;
                    if (item != null && fileName.Contains(item.id) && fileName.Contains(item.instanceID))
                    {
                        inInventory = true;
                        break;
                    }
                }
                if (inInventory) break;
            }
            if (inInventory) continue;

            // delete the file
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// Resets the database folder
    /// </summary>
    static public void ResetDatabaseFolder()
    {
        Debug.Log("Resetting database folder");
        DeleteListFromFile();
        DeleteListMetaDataFromFile();
        
        SaveListToFile();
    }

    static public void Refresh() {
        //check database
        for (int i = 0; i < database.Count; i++)
        {
            if (!database[i])
            {
                database.RemoveAt(i);
                Refresh();
                return;
            }
        }

        ValidateIDs();

        #if UNITY_EDITOR
        database.Clear();
        var guids = AssetDatabase.FindAssets("t:Item");
        for (int i = 0; i < guids.Length; i++) {
            var path = AssetDatabase.GUIDToAssetPath(guids[i]);
            var asset = AssetDatabase.LoadAssetAtPath<Item>(path);
            database.Add(asset);
        }
        #endif

        //sort by id (alphabetical with numbers at the end, :( why was this so hard to do? feels like it should be base feature)
        database.Sort((x, y) => {
            string xNumberPortion = Regex.Match(x.id, @"\d+").Value;
            string yNumberPortion = Regex.Match(y.id, @"\d+").Value;

            string xWordPortion = x.id;

            if (xNumberPortion != "") xWordPortion = x.id.Replace(xNumberPortion, "");

            string yWordPortion = y.id;

            if (yNumberPortion != "") yWordPortion = y.id.Replace(yNumberPortion, "");

            if (xWordPortion == yWordPortion)
            {
                if (xNumberPortion == "") xNumberPortion = "0";
                if (yNumberPortion == "") yNumberPortion = "0";
                return int.Parse(xNumberPortion).CompareTo(int.Parse(yNumberPortion));
            } else {
                return xWordPortion.CompareTo(yWordPortion);
            }
        });
    }

    /// <summary>
    /// Validates all item ids.
    /// </summary>
    static public void ValidateIDs() {
        #if UNITY_EDITOR
        // put selected id at the top (Reason: so that when we check for issues, it will be the first to change, in an attempt to not modify other items )
        if (DatabaseWindow.selected != null)
        {
            var selectedIndex = database.IndexOf(DatabaseWindow.selected);
            if (selectedIndex > 0 && selectedIndex < database.Count)
            {
                var selectedItem = database[selectedIndex];
                database.RemoveAt(selectedIndex);
                database.Insert(0, selectedItem);
            }
        }
        #endif

        //correct ID values
        for (int i = 0; i < database.Count; i++) {
            database[i].ValidateID();
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
