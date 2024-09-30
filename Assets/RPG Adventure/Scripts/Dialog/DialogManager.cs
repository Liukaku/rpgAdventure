using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RpgAdventure
{
    public class DialogManager : MonoBehaviour
    {
        public float timeToShowOptions = 2.0f;
        public GameObject dialogUI;
        public TextMeshProUGUI dialogHeaderText;
        public TextMeshProUGUI dialogAnswerText;
        public GameObject dialogOptionList;
        public Button dialogOptionPrefab;

        private PlayerInput m_Player;
        private QuestGiver m_Npc;
        private Dialog m_ActiveDialog;
        private float m_OptionTopPosition;
        private float m_TimerToShowOptions;

        const float c_DistanceBetweenOptions = 32.0f;

        public bool HasActiveDialog { get { return m_ActiveDialog != null; } }

        private void Start()
        {
            m_Player = PlayerInput.Instance;
        }

        private void Awake()
        {
            dialogUI.SetActive(false);
        }

        void Update()
        {
            if (!HasActiveDialog && m_Player != null && m_Player.interactTarget != null)
            {
                foreach (Collider collider in m_Player.interactTarget)
                {
                    if (collider.CompareTag("QuestGiver"))
                    {
                        m_Npc = collider.gameObject.GetComponent<QuestGiver>();
                        StartDialog();
                    }
                }

            }

            if (HasActiveDialog && Vector3.Distance(m_Player.transform.position, m_Npc.transform.position) > m_Player.distanceToInteract) 
            {
                StopDialog();
            }

            if(m_TimerToShowOptions > 0)
            {
                m_TimerToShowOptions += Time.deltaTime;
            }

            if(m_TimerToShowOptions >= timeToShowOptions)
            {
                m_TimerToShowOptions = 0;
                DisplayDialogOptioins();
            }
        }

        private void StartDialog()
        {
            m_ActiveDialog = m_Npc.dialog;
            Debug.Log("starting dialog");
            dialogHeaderText.text = m_Npc.name;
            dialogUI.SetActive(true);

            ClearDialogOptions();
            DisplayAnswerText(m_ActiveDialog.welcomeText);
            TriggerDialogOptions();
        }

        private void DisplayAnswerText(string answerText)
        {
            dialogAnswerText.gameObject.SetActive(true);
            dialogAnswerText.text = answerText;
        }

        private void DisplayDialogOptioins()
        {
            HideAnswerText();
            CreateDialogMenu();
        }

        private void TriggerDialogOptions()
        {
            m_TimerToShowOptions = 0.001f;
        }

        private void HideAnswerText()
        {
            dialogAnswerText.gameObject.SetActive(false);
        }

        private void CreateDialogMenu()
        {
            m_OptionTopPosition = 0;
            var queries = Array.FindAll(m_ActiveDialog.queries, query => !query.isAsked);

            foreach (var query in queries) 
            {
                m_OptionTopPosition += c_DistanceBetweenOptions;
                Debug.Log(query.text);
                Button DialogOption = CreateDialogOption(query.text);

            }
        }

        private Button CreateDialogOption(string dialogOptionText)
        {
            Button buttonInstance = Instantiate(dialogOptionPrefab, dialogOptionList.transform);
            buttonInstance.GetComponentInChildren<TextMeshProUGUI>().text = dialogOptionText;

            RectTransform rt = buttonInstance.GetComponent<RectTransform>();
            rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, m_OptionTopPosition, rt.rect.height);

            return buttonInstance;
        }

        private void StopDialog()
        {
            m_Npc = null;
            m_ActiveDialog = null;
            m_TimerToShowOptions = 0;
            dialogUI.SetActive(false);
        }

        private void ClearDialogOptions()
        {
            foreach(Transform child in dialogOptionList.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}