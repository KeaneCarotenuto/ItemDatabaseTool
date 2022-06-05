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
#endif

[Serializable]
public class ItemDatabase
{
    [SerializeField] public static List<Item> database = new List<Item>();

    static public void Refresh() {
        //check database
        for (int i = 0; i < database.Count; i++)
        {
            if (!database[i])
            {
                database.RemoveAt(i);
                i--;
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

    static public void ValidateIDs() {
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
