using System;
using System.Collections.Generic;
using UnityEngine;

namespace RpgAdventure
{
    public class PlayerStats : MonoBehaviour, IMessageReceiver
    {
        public int maxLevel;
        public int currentLevel;
        public int currentExperience;
        public int[] availableLevels;
        public int ExpToNextLevel
        {
            get
            {
                return availableLevels[currentLevel] - currentExperience;
            }
        }

        private void Awake()
        {
            availableLevels = new int[maxLevel];
            ComputeLevels(maxLevel);
        }

        private void ComputeLevels(int levelCount)
        {
            for (int i = 0; i < levelCount; i++)
            {
                var level = i + 1;
                var levelPow = Mathf.Pow(level, 2);
                var expToLevel = Convert.ToInt32(levelPow * levelCount);
                availableLevels[i] = expToLevel;
            }
        }

        public void OnReceiveMessage(IMessageReceiver.MessageType type, Damageable sender, Damageable.DamageMessage message)
        {
            if (type == IMessageReceiver.MessageType.DEAD)
            {
                var experience = sender.experience;
                GainExperience(experience);
            }
        }

        public void GainExperience(int gainedExp)
        {
            if (gainedExp >= ExpToNextLevel)
            {
                var expLeftOver = gainedExp - ExpToNextLevel;
                currentExperience = 0;
                currentLevel++;
                if (expLeftOver > 0)
                {
                    GainExperience(expLeftOver);
                }
            }
            else
            {
                currentExperience += gainedExp;
            }
        }
    }
}
