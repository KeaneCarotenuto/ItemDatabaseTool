using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class TestScript : MonoBehaviour
{
    // guid of this script
    #if UNITY_EDITOR 
    [ReadOnly]
    #endif 
    [SerializeField] private string guid;

    #if UNITY_EDITOR
    private void OnValidate() {
        
    }
    #endif

    public Item itemToGive;

    #if UNITY_EDITOR 
    [ReadOnly]
    #endif 
    [SerializeField] private Item m_item = null;
    public Item item
    {
        get { return m_item; }
        set
        {
            m_item = value;
            if (item.variantID == "") {
                item = item.CreateVariant();
            }
        }
    }

    public string GetSavePath()
    {
        return Application.persistentDataPath + "/" + this.GetType().Name + guid.ToString() + "/";
    }

    public string GetSaveFilePath()
    {
        return GetSavePath() + "/inventory.json";
    }

    // Update is called once per frame
    void Update()
    {
        // if S is pressed, save the item to file
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveInventory();
        }

        // if L is pressed, load the item from file
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadInventory();
        }

        // if G is pressed, give the item to the player
        if (Input.GetKeyDown(KeyCode.G))
        {
            item = itemToGive;
        }

        // if D is pressed, log ItemDatabase.database.Count
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log(ItemDatabase.database.Count);
        }
    }

    private void LoadInventory()
    {
        string fileName = System.IO.File.ReadAllText(GetSaveFilePath());

        item = Item.Load(fileName);
    }

    private void SaveInventory()
    {
        if (item == null)
        {
            Debug.Log("Item is null");
            return;
        }

        string fileName = item.id + item.variantID;
        Item.Save(fileName, item);

        // if save path doesn't exist, create it
        if (!Directory.Exists(GetSavePath()))
        {
            Directory.CreateDirectory(GetSavePath());
        }

        FileStream file = File.Create(GetSaveFilePath());
        Debug.Log("Saved to " + fileName);

        //write to file
        StreamWriter writer = new StreamWriter(file);
        writer.Write(fileName);
        writer.Close();
        file.Close();
    }
}
