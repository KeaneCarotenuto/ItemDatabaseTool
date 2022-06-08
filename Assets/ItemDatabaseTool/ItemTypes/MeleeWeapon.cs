using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class MeleeWeapon : Item
{
    [SerializeField] public float m_length = 0;

#if UNITY_EDITOR
    //Custom editor for this class
    [CustomEditor(typeof(MeleeWeapon))]
    public class WeaponEditor : ItemEditor
    {
        public override void OnInspectorGUI()
        {
            // draw base editor (Item in this case)
            base.OnInspectorGUI();

            // get the editor target
            MeleeWeapon meleeWeapon = (MeleeWeapon)target;

            // red box for weapon stats
            GUI.backgroundColor = Color.red;
            GUILayout.BeginVertical("box");
            GUI.backgroundColor = Color.white;

            // bold center text
            GUILayout.Label("Weapon Item Stats", CustomEditorStuff.center_bold_label);

            //damage
            meleeWeapon.m_length = EditorGUILayout.FloatField("Length: ", meleeWeapon.m_length);

            //end green box
            GUILayout.EndVertical();
        }
    }
#endif
}
