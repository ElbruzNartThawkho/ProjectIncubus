using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    [SerializeField] float waitTime;
    [SerializeField] Animator enemyAnim;
    [HideInInspector] public NavMeshAgent agent;
    float walkSpeed = 2f, runSpeed = 4f;
    public GameObject[] patrolLine;
    [HideInInspector] public EnemyState state = EnemyState.Patrol;
    [HideInInspector] public int count = 0;
    public bool voiceHear = false;
    public Vector3 lastLoc;
    public enum EnemyState
    {
        Patrol = 0,
        Alert = 1,
    }
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(patrolLine[count].transform.position);
    }
    private void Update()
    {
        AnimSet();
        if (state == EnemyState.Patrol)
        {
            Patrol();
        }
        else if (state == EnemyState.Alert)
        {
            agent.speed = runSpeed;
            agent.SetDestination(lastLoc);
        }
    }
    void Patrol()
    {
        if (Vector3.Distance(transform.position, patrolLine[count].transform.position) <= agent.stoppingDistance)
        {
            LatePatDes();
        }
        else if (Vector3.Distance(transform.position, lastLoc) <= agent.stoppingDistance)
        {
            agent.speed = walkSpeed;
            Invoke(nameof(GetBack), waitTime);
        }
    }
    void GetBack()
    {
        voiceHear = false;
        state = EnemyState.Patrol;
        agent.SetDestination(patrolLine[count].transform.position);
    }
    void LatePatDes()
    {
        if (count >= patrolLine.Length - 1)
        {
            count = 0;
        }
        else
        {
            count++;
        }
        agent.SetDestination(patrolLine[count].transform.position);
    }
    void AnimSet()
    {
        enemyAnim.SetFloat("IdleWalkRun", Mathf.Sqrt(Mathf.Pow(agent.velocity.x, 2) + Mathf.Pow(agent.velocity.z, 2)) / 4f);
    }
}
