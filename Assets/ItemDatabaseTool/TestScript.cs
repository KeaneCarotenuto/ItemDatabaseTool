using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class TestScript : MonoBehaviour
{
    public Item itemToGive;

    // Update is called once per frame
    void Update()
    {
        // if S is pressed, save the item to file
        if (Input.GetKeyDown(KeyCode.S))
        {
            //SaveInventory();
            GetComponent<Inventory>().SaveInventory();
        }

        // if L is pressed, load the item from file
        if (Input.GetKeyDown(KeyCode.L))
        {
            //LoadInventory();
            GetComponent<Inventory>().LoadInventory();
        }

        // if G is pressed, give the item to the player
        if (Input.GetKeyDown(KeyCode.G))
        {
            GetComponent<Inventory>().AddItem(itemToGive.id);
        }

        // if c is pressed, log ItemDatabase.database.Count
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log(ItemDatabase.database.Count);
            // find TextMeshProGUI and set it to the count
            TextMeshProUGUI text = FindObjectOfType<TextMeshProUGUI>();
            text.text += "\n" + ItemDatabase.database.Count.ToString();
        }

        // if d is pressed, save the ItemDatabase to file
        if (Input.GetKeyDown(KeyCode.D))
        {
            ItemDatabase.SaveListToFile();
        }

        // if f is pressed, load the ItemDatabase from file
        if (Input.GetKeyDown(KeyCode.F))
        {
            ItemDatabase.LoadListFromFile();
        }
    }
}
