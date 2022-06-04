using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestScript : MonoBehaviour
{
    public Item itemToSpawn;
    public Item item;
    public string itemName;

    private void Awake() {
        if (item == null) {
            InstantiateItem();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if S is pressed, save the item to file
        if (Input.GetKeyDown(KeyCode.S)) {
            Item.Save(Application.dataPath + "/Items/" + item.id + item.variantID + ".asset", item);
        }

        // if L is pressed, load the item from file
        if (Input.GetKeyDown(KeyCode.L)) {
            item = Item.Load(Application.dataPath + "/Items/" + item.id + item.variantID + ".asset");
        }
    }

    public void InstantiateItem()
    {
        item = itemToSpawn.CreateVariant();
    }
}
