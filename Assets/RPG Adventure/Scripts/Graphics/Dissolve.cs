using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RpgAdventure
{
    public class Dissolve : MonoBehaviour
    {

        public float DissovleTime = 6.0f;

        private void Awake()
        {
            DissovleTime += Time.time;
        }
        // Update is called once per frame
        void Update()
        {
            if (Time.time >= DissovleTime)
            {
                Destroy(gameObject);
            }
        }
    }

}