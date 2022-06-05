using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class TestScript : MonoBehaviour
{
    public Item itemToSpawn = null;
    public Item item = null;
    public string itemName;

    private void Awake() {
        if (item == null) {
            InstantiateItem();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if S is pressed, save the item to file
        if (Input.GetKeyDown(KeyCode.S)) {
            string path = Application.persistentDataPath + "/" + item.id + ".json";
            Item.Save(path, item);

            FileStream file = File.Create(Application.persistentDataPath + "/testScript.json");
            Debug.Log("Saved to " + path);

            //write to file
            StreamWriter writer = new StreamWriter(file);
            writer.Write(path);
            writer.Close();
            file.Close();
        }

        // if L is pressed, load the item from file
        if (Input.GetKeyDown(KeyCode.L)) {
            string itemPath = System.IO.File.ReadAllText(Application.persistentDataPath + "/testScript.json");

            item = Item.Load(itemPath);
        }
    }

    public void InstantiateItem()
    {
        string itemType = itemToSpawn.GetType().Name;
        item = itemToSpawn.CreateVariant(itemType);
    }
}
