using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    public Animator enemyAnim;
    [HideInInspector] public NavMeshAgent agent;
    //float walkSpeed = 2f, runSpeed = 4f;
    public GameObject[] patrolLine;
    [HideInInspector] public EnemyState state = EnemyState.Patrol;
    //private Transform player; public Transform hearTransform;
    [HideInInspector] public int count = 0;
    public bool voiceHear = false;
    public Vector3 lastLoc;
    public enum EnemyState
    {
        Patrol = 0,
        Alert = 1,
        GoBack = 2,
    }
    private void Awake()
    {
        //player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(patrolLine[count].transform.position);
    }
    private void Update()
    {
        Debug.Log(voiceHear + "" + state);
        if (state == EnemyState.Patrol)
        {
            Patrol();
        }
        else if (state == EnemyState.Alert)
        {
            if (voiceHear == true)
            {
                OnTheRoad();
                if (Vector3.Distance(transform.position, lastLoc) <= agent.stoppingDistance + 1)
                {
                    voiceHear = false;
                    state = EnemyState.Patrol;
                    agent.SetDestination(patrolLine[count].transform.position);
                }
            }
            else
            {
                OnTheRoad();
            }
        }
        else if (state == EnemyState.GoBack)
        {
            OnTheRoad();
        }
    }
    void Patrol()
    {
        if (Vector3.Distance(transform.position, lastLoc) <= agent.stoppingDistance + 1)
        {
            //state = EnemyState.Patrol;
            agent.SetDestination(patrolLine[count].transform.position);
        }
        else if (Vector3.Distance(transform.position, patrolLine[count].transform.position) <= agent.stoppingDistance + 1)
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
    }
    void OnTheRoad()
    {
        agent.SetDestination(lastLoc);
        if (Vector3.Distance(transform.position, lastLoc) <= agent.stoppingDistance + 1)
        {
            state = EnemyState.Patrol;
            agent.SetDestination(patrolLine[count].transform.position);
        }
    }

}
