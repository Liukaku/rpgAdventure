using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
                RegisterSlotHandler(i);
            }
        }

        private void RegisterSlotHandler(int slotIndex)
        {
            var slotBtn = inventoryPanel.GetChild(slotIndex).GetComponent<Button>();
            slotBtn.onClick.AddListener(() =>
            {
                UseItem(slotIndex);
            });
        }

        private void UseItem(int slotIndex)
        {
            var inventorySlot = GetSlotByIndex(slotIndex);

            if (inventorySlot.itemPrefab == null)
            {
                return;
            }

            PlayerController.Instance.UseItemFrom(inventorySlot);
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

        private InventorySlot GetSlotByIndex(int index)
        {
            return inventory.Find(slot => slot.index == index);
        }
    }
}