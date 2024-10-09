using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace RpgAdventure
{
    public class ItemSpawner : MonoBehaviour
    {
        public GameObject itemPrefab;
        public LayerMask targetLayer;
        public UnityEvent<GameObject> eventOnItemPickup;

        void Awake()
        {
            Instantiate(itemPrefab, transform);
            Destroy(transform.GetChild(0).gameObject);
            eventOnItemPickup.AddListener(FindObjectOfType<InventoryManager>().OnItemPickup);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (0 != (targetLayer.value << other.gameObject.layer))
            {
                eventOnItemPickup.Invoke(itemPrefab);
                //FindObjectOfType<InventoryManager>().AddItem(itemPrefab);
                Destroy(gameObject);
            }
        }

    }
}