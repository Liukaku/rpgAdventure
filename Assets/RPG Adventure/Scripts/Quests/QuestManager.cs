using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static RpgAdventure.IMessageReceiver;

namespace RpgAdventure
{
    public class QuestManager : MonoBehaviour, IMessageReceiver
    {
        public class JsonHelper
        {
            private class Wrapper<T>
            {
                public T[] array;
            }

            public static T[] GetJsonFromArray<T>(string json)
            {
                string newJson = "{\"array\": " + json + "}";
                Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
                return wrapper.array;
            }
        }

        public Quest[] quests;

        public void Awake()
        {
            LoadQuestsFromDB();
            AssignQuests();
        }

        private void LoadQuestsFromDB()
        {
            using StreamReader r = new StreamReader("Assets/RPG Adventure/DB/QuestDB.json");
            string json = r.ReadToEnd();
            var loadedQuests = JsonHelper.GetJsonFromArray<Quest>(json);
            quests = new Quest[loadedQuests.Length];
            quests = loadedQuests;
        }

        private void AssignQuests()
        {
            QuestGiver[] questGivers = FindObjectsOfType<QuestGiver>();
            if(questGivers.Length > 0 && questGivers != null)
            {
                foreach (var questGiver in questGivers)
                {
                    AssignQuestTo(questGiver);
                }
            }
        }

        private void AssignQuestTo(QuestGiver questGiver)
        {
            foreach (var quest in quests)
            {
                if(quest.questGiver == questGiver.GetComponent<UniqueId>().uid)
                {
                    questGiver.quest = quest;
                }
            }
        }

        public void OnReceiveMessage(MessageType type, Damageable sender, Damageable.DamageMessage message)
        {
            if(type == MessageType.DEAD)
            {
                CheckQuestOnEnemyDead(sender, message);
            }
        }

        private void CheckQuestOnEnemyDead(Damageable sender, Damageable.DamageMessage message)
        {
            var questLog = message.damageSource.GetComponent<QuestLog>();
            if(questLog == null ) { return; }

            foreach (var quest in questLog.quests)
            {
                if(quest.questStatus == QuestStatus.ACTIVE)
                {
                    if (quest.type == QuestType.HUNT && Array.Exists(quest.targets, (targetUid) => sender.GetComponent<UniqueId>().uid == targetUid))
                    {
                        quest.amount -= 1;
                    }

                    if (quest.amount == 0)
                    {
                        quest.questStatus = QuestStatus.COMPLETED;
                        Debug.Log("quest complete!");
                    }
                }
            }
        }
    }
}