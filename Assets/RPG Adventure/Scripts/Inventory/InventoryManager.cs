using System.Collections;
using System.Collections.Generic;
using TMPro;
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

        public void OnItemPickup(ItemSpawner spawner)
        {
            AddItemFrom(spawner);
        }

        public void AddItemFrom(ItemSpawner spawner)
        {
            var inventorySlot = GetFreeSlot();
            if (inventorySlot == null) 
            {
                return;
            }
            
            var item = spawner.itemPrefab;
            inventorySlot.Place(item);
            inventoryPanel.GetChild(inventorySlot.index)
                .GetComponentInChildren<TextMeshProUGUI>().text = item.name;
            
            Destroy(spawner.gameObject);
        }

        private InventorySlot GetFreeSlot()
        {
            return inventory.Find(slot => slot.itemName == null);
        }
    }
}