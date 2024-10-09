using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RpgAdventure
{
    [System.Serializable]
    public class InventorySlot
    {
        public int index;
        public string itemName;
        public GameObject itemPrefab;

        public InventorySlot(int index)
        {
            this.index = index;
        }

        public void Place(GameObject item)
        {
            this.itemName = item.name;
            this.itemPrefab = item;
        }
    }
}
