﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgAdventure
{
    public class InventoryManager : MonoBehaviour
    {
        public Dictionary<string, GameObject> inventory = new Dictionary<string, GameObject>();
        public int inventorySize;


        public void OnItemPickup(GameObject item)
        {
            AddItem(item);
        }

        public void AddItem(GameObject item)
        {
            if (!inventory.ContainsKey(item.name)) 
            {
                inventory.Add(item.name, item);
                Debug.Log(item.name);
                Debug.Log(inventory.Count);
            }
        }
    }
}