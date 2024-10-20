using System.Collections;
using System.Collections.Generic;
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

        private void Awake()
        {
            m_AudioSource = GetComponent<AudioSource>();
        }

        public void PlayRandomClip()
        {
            var randomPitch = Random.Range(0.7f, 1.0f);
            m_AudioSource.pitch = randomPitch;
            var clip = soundBank.clips[Random.Range(0, soundBank.clips.Length)];

            if (clip == null) 
            {
                return;
            }

            m_AudioSource.clip = clip;
            m_AudioSource.Play();
        }
    }
}
