using RpgAdventure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IAttackListener
{

    public Animator Animator { get { return m_Animator; } }

    private NavMeshAgent m_NavMeshAgent;
    private Animator m_Animator;

    private readonly int m_HashInPursuitPara = Animator.StringToHash("InPersuit");
    private readonly int m_IdlePosition = Animator.StringToHash("IdlePosition");

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        
    }

    public void HandleRotation(Transform npc, Vector3 originalPos, Vector3 banditRotation, float rotationSpeed)
    {

        if ((npc.position - originalPos).magnitude < 0.5f)
        {
            Debug.Log("rotating");
            Quaternion currentRotation = npc.rotation;
            Quaternion targetRotation = Quaternion.Euler(banditRotation);
            npc.rotation = Quaternion.Lerp(currentRotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

        }
    }

    public void ReturnHome(Transform npc, Vector3 originalPos, Vector3 banditRotation, float rotationSpeed)
    {
        FollowTarget(originalPos);
        Debug.Log("returning to home");
        HandleRotation(npc, originalPos, banditRotation, rotationSpeed);
    }

    public void FollowTarget(Vector3 targetPosition)
    {
        m_NavMeshAgent.speed = 8.0f;
        m_NavMeshAgent.acceleration = 14.0f;
        Debug.Log("navmesh enabled:" + m_NavMeshAgent.enabled);
        if (!m_NavMeshAgent.enabled) { m_NavMeshAgent.enabled = true; }
        Debug.Log("Target: " + targetPosition);
        m_NavMeshAgent.SetDestination(targetPosition);
        m_Animator.SetBool(m_HashInPursuitPara, true);
        m_Animator.SetBool(m_IdlePosition, false);
    }

    public void MeleeAttackEnd()
    {

    }

    public void MeleeAttackStart()
    {

    }
}
