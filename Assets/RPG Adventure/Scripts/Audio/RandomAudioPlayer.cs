using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace RpgAdventure
{
    public class RandomAudioPlayer : MonoBehaviour
    {
        [System.Serializable]
        public class SoundBank
        {
            public string name;
            public AudioClip[] clips;
        }

        public SoundBank soundBank = new SoundBank();
        private AudioSource m_AudioSource;
        public float randomAudioRangeMin = 0.6f;
        public float randomAudioRangeMax = 1.05f;

        public bool canPlay;
        public bool isPlaying;

        private void Awake()
        {
            m_AudioSource = GetComponent<AudioSource>();
        }

        public void PlayRandomClip(float cutOffTime)
        {
            if (m_AudioSource.time < cutOffTime && m_AudioSource.time != 0)
            {
                return;
            }

            var clip = soundBank.clips[Random.Range(0, soundBank.clips.Length)];

            if (clip == null) 
            {
                return;
            }
            var randomPitch = Random.Range(randomAudioRangeMin, randomAudioRangeMax);
            m_AudioSource.pitch = randomPitch;
            m_AudioSource.clip = clip;
            m_AudioSource.Play();

        }
    }
}
