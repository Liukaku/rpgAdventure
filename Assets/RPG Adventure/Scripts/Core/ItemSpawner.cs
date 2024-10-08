using System.Collections;
using UnityEngine;

namespace RpgAdventure
{
    public class ItemSpawner : MonoBehaviour
    {
        public GameObject itemPrefab;
        public LayerMask targetLayer;

        void Start()
        {
            Instantiate(itemPrefab, transform);
            //itemPrefab.transform.position = transform.position;
            Destroy(transform.GetChild(0).gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (0 != (targetLayer.value << other.gameObject.layer))
            {
                Destroy(gameObject);
            }
        }

    }
}