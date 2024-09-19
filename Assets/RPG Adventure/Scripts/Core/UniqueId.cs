using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace RpgAdventure
{
    public class UniqueId : MonoBehaviour
    {
        [SerializeField]
        private string m_uid = Guid.NewGuid().ToString();

        public string uid { get { return m_uid; } }
    }
}