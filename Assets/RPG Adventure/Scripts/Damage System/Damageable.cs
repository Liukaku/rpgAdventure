using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public int maxHealth = 100;

    public void ApplyDamage()
    {
        Debug.Log("Applying Damage");
    }
}
