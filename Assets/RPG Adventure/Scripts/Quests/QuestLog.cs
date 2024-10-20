﻿using System.Collections.Generic;
using UnityEngine;

namespace RpgAdventure
{
    public enum QuestStatus
    {
        ACTIVE,
        FAILED,
        COMPLETED
    }
    [System.Serializable]
    public class AcceptedQuest: Quest
    {
        public QuestStatus questStatus;
        public AcceptedQuest(Quest quest)
        {
            uid = quest.uid;
            title = quest.title;
            description = quest.description;
            experience = quest.experience;
            gold = quest.gold;
            amount = quest.amount;
            targets = quest.targets;
            talkTo = quest.talkTo;
            explore = quest.explore;
            questGiver = quest.questGiver;
            type = quest.type;
            questStatus = QuestStatus.ACTIVE;
        }
    }
    public class QuestLog : MonoBehaviour
    {
            public List<AcceptedQuest> quests = new List<AcceptedQuest>();
            public void AddQuest(Quest quest)
            {
                Debug.Log("new quest: " + quest.title);
                quests.Add(new AcceptedQuest(quest));
            }
    }

}