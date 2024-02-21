using RpgAdventure;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayerScanner
{
    public float meleeDetectionRadius = 5.0f;
    public float detectionRange = 10.0f;
    public float detectionAngle = 90.0f;

    public void SetDetectionAngle(float detectionAngle)
    {
        this.detectionAngle = detectionAngle;
    }

    public PlayerController DetectPlayer(Transform detector)
    {
        if (PlayerController.Instance == null)
        {
            return null;
        }

        Vector3 enemyPosition = detector.position;
        Vector3 disToPlayer = PlayerController.Instance.transform.position - enemyPosition;
        disToPlayer.y = 0; // to make debugging easier 

        // check if the player is within a general range
        if (disToPlayer.magnitude <= detectionRange)
        {
            // check if the player is within the angle specified in decectionAngle
            if ((Vector3.Dot(disToPlayer.normalized, detector.forward) >
                Mathf.Cos(detectionAngle * 0.5f * Mathf.Deg2Rad)) || 
                disToPlayer.magnitude <= meleeDetectionRadius)
            {
                //Debug.Log("player detected");
                return PlayerController.Instance;
            }
        }
        return null;
    }
}
