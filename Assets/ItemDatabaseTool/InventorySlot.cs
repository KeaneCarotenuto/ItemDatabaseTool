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
public class InventorySlot : UnityEngine.Object
{
    // guid of this script
    #if UNITY_EDITOR
    [ReadOnly]
    #endif
    [SerializeField] private string guid;

    // type filter
    [SerializeField] public List<System.Type> typeFilter = new List<System.Type>();

    [SerializeField] public Item m_item;
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

            property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(position, property.isExpanded, label);

            if (property.isExpanded){
                // Calculate rects
                Rect itemRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, 50, position.height / 4);
                Rect typeButtonRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2.0f, 50, position.height / 4);

                // draw the item
                inventorySlot.item = (Item)EditorGUI.ObjectField(itemRect, inventorySlot.item, typeof(Item), false);


                EditorGUI.FloatField(typeButtonRect, 2);
                // testing
            }

            EditorGUI.EndFoldoutHeaderGroup();

            EditorGUI.indentLevel = indent;

            // end drawing the property
            EditorGUI.EndProperty();

            EditorUtility.SetDirty(property.serializedObject.targetObject);
            
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight + // the foldout
                    (3) *      // for each related obj:
                    (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
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