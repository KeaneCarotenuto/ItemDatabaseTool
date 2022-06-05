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
[Serializable]
public class RangedWeapon : Item
{
    [SerializeField] public float m_damage = 0;

    public override Item CreateVariant(string _type)
    {
        RangedWeapon newItem = (RangedWeapon)base.CreateVariant(_type);

        // make RangedWeapon specific changes
        newItem.m_damage = m_damage;


        return newItem;
    }

    #if UNITY_EDITOR
    //inspector gui stuff
    [CustomEditor(typeof(RangedWeapon))]
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
            ((RangedWeapon)target).m_damage = EditorGUILayout.FloatField(((RangedWeapon)target).m_damage);
            GUILayout.EndHorizontal();

            //end green box
            GUILayout.EndVertical();
        }
    }
    #endif
            
}