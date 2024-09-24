using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RpgAdventure
{
    public class DialogManager : MonoBehaviour
    {
        public GameObject dialogUI;
        public TextMeshProUGUI dialogHeaderText;

        private bool m_hasActiveDialog;

        private void Awake()
        {
            dialogUI.SetActive(false);
        }

        void Update()
        {
            if (!m_hasActiveDialog && PlayerInput.Instance != null && PlayerInput.Instance.interactTarget != null)
            {
                foreach (Collider collider in PlayerInput.Instance.interactTarget)
                {
                    if (collider.CompareTag("QuestGiver"))
                    {
                        StartDialog();
                    }
                }

            }
        }

        private void StartDialog()
        {
            m_hasActiveDialog = true;
            Debug.Log("starting dialog");
            dialogUI.SetActive(true);
            dialogHeaderText.text = "Hello from code";
        }
    }
}