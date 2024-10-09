using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgAdventure
{
    public class InventoryManager : MonoBehaviour
    {
        public Transform inventoryPanel;
        public List<InventorySlot> inventory = new List<InventorySlot>();

        private int m_inventorySize;

        private void Awake()
        {
            m_inventorySize = inventoryPanel.childCount;
            CreateInventory(m_inventorySize);
        }

        private void CreateInventory(int inventorySize)
        {
            for (int i = 0; i < inventorySize; i++)
            {
                inventory.Add(new InventorySlot(i));
            }
        }

        public void OnItemPickup(GameObject item)
        {
            AddItem(item);
        }

        public void AddItem(GameObject item)
        {
            var inventorySlot = GetFreeSlot();
            if (inventorySlot == null) 
            {
                Debug.Log("inventory is full");
                return;
            }

            inventorySlot.Place(item);
            Debug.Log("added: " + item.name);
        }

        private InventorySlot GetFreeSlot()
        {
            return inventory.Find(slot => slot.itemName == null);
        }
    }
}