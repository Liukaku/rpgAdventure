using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
        private bool m_ForceDialogQuit;

        const float c_DistanceBetweenOptions = 55.0f;

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

                if(m_TimerToShowOptions >= timeToShowOptions)
                {
                    m_TimerToShowOptions = 0;
                    if (m_ForceDialogQuit)
                    {
                        StopDialog();
                    } else
                    {
                        DisplayDialogOptioins();
                    }

                }
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

            Debug.Log("E");
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

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
                RegisterOptionClickHandler(DialogOption, query);
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

        private void RegisterOptionClickHandler(Button DialogOption, DialogQuery query)
        {
            EventTrigger trigger = DialogOption.gameObject.AddComponent<EventTrigger>();
            var pointerDown = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };

            pointerDown.callback.AddListener((e) =>
            {
                if (!String.IsNullOrEmpty(query.answer.questId))
                {
                    m_Player.GetComponent<QuestLog>().AddQuest(m_Npc.quest);
                }
                if(query.answer.forcedDialogQuit)
                {
                    m_ForceDialogQuit = true;
                }
                if (!query.isAlwaysAsked)
                {
                    query.isAsked = true;
                }
                ClearDialogOptions();
                DisplayAnswerText(query.answer.text);
                TriggerDialogOptions();
            });

            trigger.triggers.Add(pointerDown);
        }

        private void StopDialog()
        {
            m_Npc = null;
            m_ActiveDialog = null;
            m_TimerToShowOptions = 0;
            dialogUI.SetActive(false);
            Debug.Log("no E");
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            m_ForceDialogQuit = false;
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