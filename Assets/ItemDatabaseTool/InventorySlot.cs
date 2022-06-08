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
    enum itemType{};

    [Serializable]
    public class smallClass{
        public int a = 0;
        public int b = 0;
    }

    [SerializeField] public smallClass smallClassInstance;

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

    // private void OnValidate() {
    //     itemType.
    // }
}