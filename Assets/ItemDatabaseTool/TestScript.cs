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
    public TMP_InputField consoleInput;
    public TextMeshProUGUI consoleLog;

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

        // if c is pressed, log ItemDatabase.database.Count
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log(ItemDatabase.database.Count);
            // find TextMeshProGUI and set it to the count
            Log("ItemDatabase.database.Count: " + ItemDatabase.database.Count);
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

        // if enter is pressed, try send command
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!consoleInput) return;
            string text = consoleInput.text;
            if (text == "") return;

            TrySendCommand(text);

            consoleInput.text = "";
        }
    }

    public void TrySendCommand(string _command){
        Log(_command);

        // e.g. "give item_id amount"
        string[] split = _command.Split(' ');
        if (split.Length >= 2 && split[0] == "give")
        {
            string itemID = split[1];
            int amount = split.Length >= 3 ? int.Parse(split[2]) : 1;

            // give the item to the player
            if (GetComponent<Inventory>().AddItemToInventory(itemID, amount)){
                Log("- You have received " + itemID);
            }
            else{
                Log("- Could not give " + itemID);
            }
        }
        else{
            Log("- Invalid command");
        }
    }

    public void Log(string _text)
    {
        if (!consoleLog) return;

        consoleLog.text += "\n" + _text + "\n";
    }
}