using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RpgAdventure
{
    public class QuestManager : MonoBehaviour
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
        }

        private void LoadQuestsFromDB()
        {
            using (StreamReader r = new StreamReader("Assets/RPG Adventure/DB/QuestDB.json"))
            {
                string json = r.ReadToEnd();
                var loadedQuests = JsonHelper.GetJsonFromArray<Quest>(json);
                quests = new Quest[loadedQuests.Length];
                quests = loadedQuests;
                Debug.Log(quests);
            }
        }

    }
}