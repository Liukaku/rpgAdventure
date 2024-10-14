using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RpgAdventure
{
    public class HudManager : MonoBehaviour
    {
        public Slider healthSlider;

        public void SetMaxHealth(int health)
        {
            healthSlider.maxValue = health;
            SetHealth(health);
        }

        public void SetHealth(int health)
        {
            healthSlider.value = health;
        }
    }
}