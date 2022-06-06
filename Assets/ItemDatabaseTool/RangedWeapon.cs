using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class RangedWeapon : Item
{
    [SerializeField] public float m_damage = 0;

    public override Item CreateVariant()
    {
        // Create base item
        RangedWeapon newItem = (RangedWeapon)base.CreateVariant();

        // make RangedWeapon specific changes
        newItem.m_damage = m_damage;

        return newItem;
    }

    #if UNITY_EDITOR
    //Custom editor for this class
    [CustomEditor(typeof(RangedWeapon))]
    public class WeaponEditor : ItemEditor
    {
        public override void OnInspectorGUI()
        {
            // draw base editor (Item in this case)
            base.OnInspectorGUI();

            // get the editor target
            RangedWeapon rangedWeapon = (RangedWeapon)target;

            // red box for weapon stats
            GUI.backgroundColor = Color.red;
            GUILayout.BeginVertical("box");
            GUI.backgroundColor = Color.white;

            // bold center text
            GUILayout.Label("Weapon Item Stats", CustomEditorStuff.center_bold_label);

            //damage
            rangedWeapon.m_damage = EditorGUILayout.FloatField("Damage: ", rangedWeapon.m_damage);

            //end green box
            GUILayout.EndVertical();
        }
    }
    #endif
            
}