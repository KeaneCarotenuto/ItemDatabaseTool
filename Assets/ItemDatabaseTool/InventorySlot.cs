using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using TMPro;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class InventorySlot
{
    // guid of this script
    #if UNITY_EDITOR
    [ReadOnly]
    #endif
    [SerializeField] private string guid = "";

    // type filter
    [SerializeField] public List<string> typeFilter = new List<string>();

    [SerializeField] public Item m_item = null;
    [SerializeField] public Item item
    {
        get { return m_item; }
        set
        {
            if (value != null)
            {
                m_item = value;
                if (item.variantID == "")
                {
                    item = item.CreateVariant();
                }
            }
        }
    }

    // custom property drawer for this class
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(InventorySlot))]
    public class InventorySlotDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InventorySlot inventorySlot = (InventorySlot)fieldInfo.GetValue(property.serializedObject.targetObject);

            // begin drawing the property
            EditorGUI.BeginProperty(position, label, property);

            // draw the label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            Rect itemRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            Rect typeButtonRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3.0f, position.width, EditorGUIUtility.singleLineHeight);

            // draw the item
            inventorySlot.item = (Item)EditorGUI.ObjectField(itemRect, inventorySlot.item, typeof(Item), false);


            // dropdown for item type
            // array of possible types
            string[] names = Item.AllTypes.Select(t => t.Name).ToArray();
            names = names.Concat(new string[] { typeof(Item).Name }).ToArray();

            // display the GenericMenu when pressing a button
            if (GUI.Button(new Rect(position.x, position.y + 50, position.width, EditorGUIUtility.singleLineHeight), "Type Filter"))
            {

                // draw the dropdown
                GenericMenu menu = new GenericMenu();
                for (int i = 0; i < names.Length; i++)
                {
                    string name = names[i];
                    menu.AddItem(new GUIContent(name), inventorySlot.typeFilter.Contains(name), () => { 
                        inventorySlot.typeFilter.Add(name); 
                        // save
                        EditorUtility.SetDirty(property.serializedObject.targetObject);
                        });
                }

                // draw the dropdown
                menu.ShowAsContext();
            }

            EditorGUI.EndFoldoutHeaderGroup();

            EditorGUI.indentLevel = indent;

            // end drawing the property
            EditorGUI.EndProperty();

            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }

        public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
 
            SerializedObject childObj = new UnityEditor.SerializedObject(property.serializedObject.targetObject);
            SerializedProperty ite = childObj.GetIterator();
    
            float totalHeight = EditorGUI.GetPropertyHeight (property, label, true) + EditorGUIUtility.standardVerticalSpacing;
    
            while (ite.NextVisible(true))
            {
                totalHeight += EditorGUI.GetPropertyHeight(ite, label, true) + EditorGUIUtility.standardVerticalSpacing;
            }
 
            return totalHeight;
        }
    }
    #endif
}


// // dropdown for item type
//             // array of possible types
//             string[] names = Item.AllTypes.Select(t => t.Name).ToArray();
//             names = names.Concat(new string[] { typeof(Item).Name }).ToArray();

//             // array of current types
//             string[] currentTypes = inventorySlot.typeFilter.Select(t => t.Name).ToArray();

//             // display the GenericMenu when pressing a button
//             if (GUI.Button(new Rect(position.x, position.y + 20, position.width, position.height), "Types"))
//             {

//                 // draw the dropdown
//                 GenericMenu menu = new GenericMenu();
//                 for (int i = 0; i < names.Length; i++)
//                 {
//                     string name = names[i];
//                     menu.AddItem(new GUIContent(name), currentTypes.Contains(name), () => { inventorySlot.typeFilter.Add(System.Type.GetType(name)); });
//                 }

//                 // draw the dropdown
//                 menu.ShowAsContext();
//             }