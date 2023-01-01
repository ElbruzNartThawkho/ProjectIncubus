using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    [SerializeField] float waitTime;
    public Animator enemyAnim;

    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public EnemyState state = EnemyState.Patrol;
    [HideInInspector] public int count = 0;


    public GameObject caution, alarm, arrow;
    public GameObject[] patrolLine;
    public Vector3 lastLoc;
    public EnemyType enemyType;
    public bool voiceHear = false, shotState = false;


    float walkSpeed = 2f, runSpeed = 4f;

    public enum EnemyType
    {
        meleeFighter = 0,
        ranger = 1,
    }

    public enum EnemyState
    {
        Patrol = 0,
        Alert = 1
    }
    private void Awake()
    {
        if (enemyType == EnemyType.ranger)
        {
            walkSpeed = 4f; runSpeed = 8f;
        }
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;
        agent.SetDestination(patrolLine[count].transform.position);
    }
    private void Update()
    {
        AnimSet();
        if (state is EnemyState.Patrol)
        {
            Patrol();
        }
        else if (state is EnemyState.Alert)
        {
            agent.speed = runSpeed;
            Quaternion lookOnLook = Quaternion.LookRotation(lastLoc - transform.position); transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, Time.deltaTime * 5f);
            //agent.transform.LookAt(new Vector3(lastLoc.x, transform.position.y, lastLoc.z));
            if (voiceHear == true)
            {
                if (alarm.activeSelf == false)
                {
                    caution.SetActive(true);
                }
                agent.SetDestination(lastLoc);
                if (Vector3.Distance(transform.position, lastLoc) <= agent.stoppingDistance + 1)
                {
                    voiceHear = false;
                    state = EnemyState.Patrol;
                }
            }
            else
            {
                alarm.SetActive(true); caution.SetActive(false);
                //düþman tipine göre menzile girince ateþ etmeye baþlayacak
                if (enemyType is EnemyType.ranger)
                {
                    agent.SetDestination(transform.position);
                    if (shotState is false && enemyAnim.GetCurrentAnimatorStateInfo(0).IsName("ReleaseArrow") is false)
                    {
                        shotState = true;
                        enemyAnim.SetTrigger("ArrowShot");
                    }
                    else if (enemyAnim.GetCurrentAnimatorStateInfo(0).IsName("ReleaseArrow") && shotState)
                    {
                        Instantiate(arrow, transform.position + transform.forward + transform.up/2, transform.rotation).GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
                        shotState = false;
                    }
                }
                else
                {
                    agent.SetDestination(lastLoc);
                }
            }
        }
    }
    void Patrol()
    {
        if (Vector3.Distance(transform.position, patrolLine[count].transform.position) <= agent.stoppingDistance)
        {
            caution.SetActive(false); alarm.SetActive(false);
            LatePatDes();
        }
        else if (Vector3.Distance(transform.position, lastLoc) <= agent.stoppingDistance)
        {
            alarm.SetActive(false);
            caution.SetActive(true);
            Invoke(nameof(GetBack), waitTime);
        }
    }
    void GetBack()
    {
        voiceHear = false;
        agent.speed = walkSpeed;
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
