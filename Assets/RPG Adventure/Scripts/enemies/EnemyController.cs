using RpgAdventure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IAttackListener
{

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

    public void FollowTarget(Vector3 targetPosition)
    {
        m_NavMeshAgent.speed = 8.0f;
        m_NavMeshAgent.acceleration = 14.0f;
        if (!m_NavMeshAgent.enabled) { m_NavMeshAgent.enabled = true; }

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
