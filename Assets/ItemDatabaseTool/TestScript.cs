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

    private List<string> commandHistory = new List<string>();
    private int upIndex = 0;

    // Update is called once per frame
    void Update()
    {
        // if console input is not selected
        if (!consoleInput.isFocused){
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

            // d to destroy inventory items
            if (Input.GetKeyDown(KeyCode.D))
            {
                // destroy all items in the inventory
                for (int i = 0; i < GetComponent<Inventory>().slots.Count; i++)
                {
                    Item item = GetComponent<Inventory>().slots[i].item;
                    if (item != null)
                    {
                        item.DestroyInstance();
                    }
                }
            }
        }
        

        // if enter is pressed, try send command
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!consoleInput) return;
            string text = consoleInput.text;
            if (text == "") return;

            TrySendCommand(text);

            consoleInput.text = "";

            StartTyping();
        }

        // if up arrow is pressed, go to previous command
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            upIndex--;
            if (upIndex < 0) upIndex = 0;
            if (commandHistory.Count > 0)
            {
                consoleInput.text = commandHistory[upIndex];
            }

            StartTyping();
        }

        // if down arrow is pressed, go to next command
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            upIndex++;
            if (upIndex > commandHistory.Count - 1) upIndex = commandHistory.Count - 1;
            if (commandHistory.Count > 0)
            {
                consoleInput.text = commandHistory[upIndex];
            }

            StartTyping();
        }
    }

    public void StartTyping()
    {
        // select the box, start typing, and move cursor to end
        consoleInput.Select();
        consoleInput.ActivateInputField();
        consoleInput.caretPosition = consoleInput.text.Length;
    }

    public void TrySendCommand(string _command){
        Log(_command);

        commandHistory.Add(_command);

        upIndex = commandHistory.Count;

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