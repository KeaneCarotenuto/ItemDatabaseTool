using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class Inventory : MonoBehaviour
{
    static public List<Inventory> allInventories = new List<Inventory>();

    // constructor
    public Inventory()
    {
        AddInvetory(this);
    }

    // destructor
    ~Inventory()
    {
        RemoveInvetory(this);
    }

    [Serializable]
    public class InventorySlot
    {
        // type filter
        [SerializeField] public List<string> typeFilter = new List<string>();

        [SerializeField] private Item m_item = null;
        [SerializeField] public Item item
        {
            get { return m_item; }
            set
            {
                // if same, return
                if (m_item == value) return;
                
                m_item = value;

                // if null, return
                if (m_item == null) return;

                // if item type is not in filter, clear item
                if (typeFilter.Count() > 0 && !typeFilter.Contains(m_item.GetType().Name)) {
                    m_item = null;

                    Debug.LogWarning("Item type does not match inventory type filter.");

                    return;
                }

                if (m_item.variantID == "")
                {
                    m_item = m_item.CreateVariant();
                }
            }
        }
    }

    [SerializeField] private string m_id = "";
    [SerializeField] public string id
    {
        get { return m_id; }
        set
        {
            m_id = value;
            Inventory.ValidateIDs(this);
        }
    }

    [SerializeField] private List<InventorySlot> m_slots = new List<InventorySlot>();

    /// <summary>
    /// Add an item to the inventory by reference.
    /// </summary>
    public void AddItem(Item _item){
        // if item is null, return
        if (_item == null) return;

        // if item is in inventory, add to stack
        if (m_slots.Any(x => x.item != null && x.item.id == _item.id && x.item.variantID == _item.variantID)) {
            InventorySlot slot = m_slots.Find(x => x.item != null && x.item.id == _item.id && x.item.variantID == _item.variantID);
            
            slot.item.TryAddToStack(_item);

            return;
        }

        // if item is not in inventory, try to add to empty valid slot
        if (m_slots.Any(x => x.item == null && x.typeFilter.Count() <= 0 || (x.typeFilter.Count() > 0 && x.typeFilter.Contains(_item.GetType().Name)))) {
            InventorySlot slot = m_slots.Find(x => x.item == null && x.typeFilter.Count() <= 0 || (x.typeFilter.Count() > 0 && x.typeFilter.Contains(_item.GetType().Name)));

            slot.item = _item;

            return;
        }
        else{
            Debug.LogWarning("No empty valid slot found.");
            return;
        }
    }

    /// <summary>
    /// Tries to add an item to the inventory by ID.
    /// </summary>
    public void AddItem(string _id){
        Item item = ItemDatabase.GetItem(_id);

        AddItem(item);
    }

    public string GetSavePath()
    {
        return Application.dataPath + "/ItemDatabase/" + this.GetType().Name + id.ToString() + "/";
    }

    public string GetSaveFilePath()
    {
        return GetSavePath() + "/inventory.json";
    }

    public void SaveInventory()
    {
        // if save path doesn't exist, create it
        if (!Directory.Exists(GetSavePath()))
        {
            Directory.CreateDirectory(GetSavePath());
        }

        List<string> fileNames = new List<string>();

        foreach (InventorySlot slot in m_slots)
        {
            if (slot.item != null)
            {
                string fileName = slot.item.id + slot.item.variantID + ".json";
                Item.Save(Item.GetVariantSavePath(), fileName, slot.item);

                fileNames.Add(fileName);
            }
        }

        FileStream file = File.Create(GetSaveFilePath());

        //write to file
        StreamWriter writer = new StreamWriter(file);
        foreach (string fileName in fileNames)
        {
            writer.WriteLine(fileName);
        }
        writer.Close();
        file.Close();
    }

    public void LoadInventory()
    {
        string file = System.IO.File.ReadAllText(GetSaveFilePath());

        string[] fileNames = file.Split('\n');

        for (int i = 0; i < fileNames.Length; i++)
        {
            string fileName = fileNames[i];

            // remove newline
            fileName = fileName.Replace("\n", "");

            // remove whitespace
            fileName = fileName.Trim();

            if (fileName == "") continue;

            Item item = Item.Load(Item.GetVariantSavePath(), fileName);

            if (item == null)
            {
                Debug.LogWarning("Item " + fileName + " could not be loaded. NULL");
                continue;
            }

            // for loading, we need to directly set the item
            if (m_slots.Count() > i)
            {
                m_slots[i].item = item;
            }
            else
            {
                Debug.LogWarning("Item " + fileName + " could not be loaded. NO SPACE");
            }
        }
    }

    public void ValidateID(){
        //correct id
        //to lower
        m_id = m_id.ToLower();
        //replace non letters and numbers with ""
        m_id = Regex.Replace(m_id, @"[^a-zA-Z0-9_]", "");
        //replace space with _
        m_id = m_id.Replace(" ", "_");
    }


    #if UNITY_EDITOR
    private void OnValidate() {
        if (m_id == "") {
            m_id = System.Guid.NewGuid().ToString();

            ValidateID();

            // set dirty
            EditorUtility.SetDirty(this);
        }
    }

    public static void AddInvetory(Inventory inventory)
    {
        if (allInventories.Contains(inventory))
        {
            return;
        }
        allInventories.Add(inventory);
    }

    public static void RemoveInvetory(Inventory inventory)
    {
        if (!allInventories.Contains(inventory))
        {
            return;
        }
        allInventories.Remove(inventory);
    }

    static public void ValidateIDs(Inventory _selectedInventory = null) {
        #if UNITY_EDITOR
        // put selected id at the top (Reason: so that when we check for issues, it will be the first to change, in an attempt to not modify other items )
        if (_selectedInventory != null)
        {
            var selectedIndex = allInventories.IndexOf(_selectedInventory);
            if (selectedIndex > 0 && selectedIndex < allInventories.Count)
            {
                var selectedItem = allInventories[selectedIndex];
                allInventories.RemoveAt(selectedIndex);
                allInventories.Insert(0, selectedItem);
            }
        }
        #endif

        //correct ID values
        for (int i = 0; i < allInventories.Count; i++) {
            allInventories[i].ValidateID();
        }

        //correct duplicate IDs
        for (int i = 0; i < allInventories.Count; i++) {
            for (int j = 0; j < allInventories.Count; j++) {
                if (allInventories[i].id == allInventories[j].id && i != j) {
                    //check if last char is a number
                    if (char.IsNumber(allInventories[i].id[allInventories[i].id.Length - 1])) {
                        //go backwards and count numbers
                        int count = 0;
                        for (int k = allInventories[i].id.Length - 1; k >= 0; k--) {
                            if (char.IsNumber(allInventories[i].id[k])) {
                                count++;
                            } else {
                                break;
                            }
                        }
                        //add one to the number
                        int number = int.Parse(allInventories[i].id.Substring(allInventories[i].id.Length - count, count)) + 1;
                        //remove last numbers
                        allInventories[i].id = allInventories[i].id.Substring(0, allInventories[i].id.Length - count);
                        //add new number
                        allInventories[i].id += number;

                        ValidateIDs();
                    }
                    else {
                        //add _1 to the number
                        allInventories[i].id = allInventories[i].id + "_1";
                    }
                }
            }
        }
    }
    


    // custom editor
    [CustomEditor(typeof(Inventory))]
    public class InventoryEditor : Editor
    {
        static int objectSelectorIndex = -1;

        public override void OnInspectorGUI()
        {
            //DrawDefaultInspector();

            Inventory inventory = (Inventory)target;

            //id
            inventory.id = EditorGUILayout.TextField(new GUIContent("ID","A unique identifier for this inventory"), inventory.id);

            // space
            EditorGUILayout.Space();

            // horiz
            EditorGUILayout.BeginHorizontal();
            //slots 
            EditorGUILayout.LabelField("Slots [" + inventory.m_slots.Count + "]", CustomEditorStuff.bold_label);
            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < inventory.m_slots.Count; i++)
            {
                InventorySlot slot = inventory.m_slots[i];

                EditorGUILayout.BeginVertical("box");

                // horiz
                EditorGUILayout.BeginHorizontal();
                // labal for index
                EditorGUILayout.LabelField("Slot " + i, GUILayout.Width(50));

                //item
                slot.item = (Item)EditorGUILayout.ObjectField(slot.item, typeof(Item), false);

                //type filter
                // dropdown for item type
                // array of possible types
                string[] names = Item.AllTypes.Select(t => t.Name).ToArray();
                names = names.Concat(new string[] { typeof(Item).Name }).ToArray();
                // remove "item" from names
                names = names.Where(n => n != typeof(Item).Name).ToArray();
                // remove "item" from type filter
                slot.typeFilter = slot.typeFilter.Where(t => t != typeof(Item).Name).ToList();

                // display the GenericMenu when pressing a button
                int filterCount = slot.typeFilter.Count();
                if (GUILayout.Button(new GUIContent(filterCount == 1 ? slot.typeFilter[0] : filterCount > 1 ? "[Multiple]" : "Item", "Ticked types are allowed in this slot"), GUILayout.MinWidth(120)))
                {
                    // draw the dropdown
                    GenericMenu menu = new GenericMenu();
                    for (int j = 0; j < names.Length; j++)
                    {
                        string name = names[j];
                        menu.AddItem(new GUIContent(name), slot.typeFilter.Contains(name), () => { 
                            if (slot.typeFilter.Contains(name)) {
                                slot.typeFilter.Remove(name);
                            } else {
                                slot.typeFilter.Add(name);
                            }
                        });
                    }

                    // draw the dropdown
                    menu.ShowAsContext();
                }

                // remove button
                if (GUILayout.Button(new GUIContent("X", "Remove this slot"), GUILayout.Width(20)))
                {
                    inventory.m_slots.RemoveAt(i);
                    break;
                }
                
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();  
            }
            if (GUILayout.Button("Add Slot"))
            {
                inventory.m_slots.Add(new InventorySlot());
            }

            // save on change
            if (GUI.changed)
            {
                EditorUtility.SetDirty(inventory);
            }
        }
    }
    #endif
}