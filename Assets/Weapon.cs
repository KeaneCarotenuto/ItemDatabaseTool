using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
using System.Linq;

[CreateAssetMenu(fileName = "Weapon", menuName = "Item/New Weapon")]
#endif
public class Weapon : Item
{
    public float damage = 0;

    #if UNITY_EDITOR
    //inspector gui stuff
    [CustomEditor(typeof(Weapon))]
    public class WeaponEditor : ItemEditor
    {
        public override void OnInspectorGUI()
        {
            DrawUI();

            GUI.backgroundColor = Color.red;
            //green box for item
            GUILayout.BeginVertical("box");
            // bold text
            GUILayout.Label("Weapon Item Stats", CustomEditorStyles.center_bold_label);
            GUI.backgroundColor = Color.white;

            //damage
            GUILayout.BeginHorizontal();
            GUILayout.Label("Damage: ");
            ((Weapon)target).damage = EditorGUILayout.FloatField(((Weapon)target).damage);
            GUILayout.EndHorizontal();

            //end green box
            GUILayout.EndVertical();
        }
    }
    #endif
            
}