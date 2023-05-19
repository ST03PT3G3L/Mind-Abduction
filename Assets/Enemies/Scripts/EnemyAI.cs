using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SocialPlatforms;
using Unity.VisualScripting;
using System.Diagnostics;
using TMPro;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] Rigidbody target;
    [SerializeField] GameObject player;
    [SerializeField] int callAssistanceRadius = 40;
    /*
    [SerializeField] int confusionMin;
    [SerializeField] int confusionMax;
    */
    [SerializeField] float chaseRangeMax = 18f;
    [SerializeField] int confusionDuration = 5;
    [SerializeField] float chaseRangeMin = 5f;
    [SerializeField] float turnSpeed = 20f;
    NavMeshAgent navMeshAgent;
    float distanceToPlayer = int.MaxValue;
    public bool isProvoked = false;
    Stopwatch waitingTimer = new Stopwatch();
    public bool calledAssistance = false;
    EnemyAttack enemyAttack;

    Vector3 speed = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        enemyAttack = transform.GetComponent<EnemyAttack>();
                navMeshAgent = GetComponent<NavMeshAgent>();
        distanceToPlayer = Vector3.Distance(target.transform.position, transform.position);
    }

    public float GetDistanceToPlayer
    {
       get { return this.distanceToPlayer; }
    }
    void OnEnable()
    {
        gameObject.tag = "Enemy";
        gameObject.GetComponent<PlayerMovement2>().enabled = false;
        gameObject.GetComponent<Sliding>().enabled = false;
        gameObject.GetComponentInChildren<Camera>().enabled = false;
        gameObject.GetComponentInChildren<PlayerLook>().enabled = false;
        gameObject.GetComponent<MindAbduction>().enabled = false;
        gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        gameObject.GetComponent<CapsuleCollider>().enabled = true;
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        gameObject.GetComponent<EnemyAttack>().enabled = true;
        gameObject.GetComponent<WaypointMover>().enabled = true;
    }



    public void SetTarget()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
     /*   UnityEngine.Debug.DrawRay(transform.position, transform.forward * 18, Color.red, 2, false);
        UnityEngine.Debug.DrawRay(transform.position, Quaternion.AngleAxis(-15, new Vector3(0, 1, 0)) * transform.forward * 18, Color.red, 2, false);
        UnityEngine.Debug.DrawRay(transform.position, Quaternion.AngleAxis(-7.5f, new Vector3(0, 1, 0)) * transform.forward * 18, Color.red, 2, false);
        UnityEngine.Debug.DrawRay(transform.position, Quaternion.AngleAxis(7.5f, new Vector3(0, 1, 0)) * transform.forward * 18, Color.red, 2, false); */


        distanceToPlayer = Vector3.Distance(target.transform.position, transform.position);
        CheckIfProvokedByRangeAndSpeed();
        if (isProvoked)
        {
            CallAssistance();
            FaceTarget();
            navMeshAgent.SetDestination(target.transform.position);
            if (distanceToPlayer > chaseRangeMax && !IfOtherEnemiesAreWithinProvokingRange())
            {
                InitializeUnprovoking();
            }
            if (distanceToPlayer <= navMeshAgent.stoppingDistance)
            {
                GetComponent<Animator>().SetBool("Attack Trigger", true);
                enemyAttack.couldShoot = true;
            }
            else
            {
                GetComponent<Animator>().SetBool("Attack Trigger", false);
                enemyAttack.couldShoot = false;
            }
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, chaseRangeMax) || 
            Physics.Raycast(transform.position, Quaternion.AngleAxis(-15, new Vector3(0, 1, 0)) * transform.forward, chaseRangeMax) ||
            Physics.Raycast(transform.position, Quaternion.AngleAxis(15, new Vector3(0, 1, 0)) * transform.forward, chaseRangeMax) ||
            Physics.Raycast(transform.position, Quaternion.AngleAxis(-10f, new Vector3(0, 1, 0)) * transform.forward, chaseRangeMax) ||
            Physics.Raycast(transform.position, Quaternion.AngleAxis(10, new Vector3(0, 1, 0)) * transform.forward, chaseRangeMax) ||
            Physics.Raycast(transform.position, Quaternion.AngleAxis(-5f, new Vector3(0, 1, 0)) * transform.forward, chaseRangeMax) ||
            Physics.Raycast(transform.position, Quaternion.AngleAxis(-5f, new Vector3(0, 1, 0)) * transform.forward, chaseRangeMax))
        {
            if (hit.transform != null) {
                Rigidbody result = hit.transform.GetComponent<Rigidbody>();
                if (result != null)
                {
                    ProvokeEnemy();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        speed = target.velocity;

    }

    public void InitializeUnprovoking()
    {
        int timeForMoving = 6000;
        int timeForForgetting = 10000;
        if (!waitingTimer.IsRunning)
        {
            waitingTimer.Start();
        }
        if (waitingTimer.ElapsedMilliseconds < new System.Random().Next(timeForMoving - 900, timeForMoving + 900))
        {
            navMeshAgent.speed = 2.5f;
            Vector3.MoveTowards(transform.position, transform.forward, 15);
        }
        else
        {
            navMeshAgent.speed = 0;
            GetComponent<Animator>().SetTrigger("Confused Trigger");
        }
        if (waitingTimer.ElapsedMilliseconds > new System.Random().Next(timeForForgetting - 1200, timeForForgetting + 1200))
        {
            isProvoked = false;
            calledAssistance = false;
            GetComponent<Animator>().SetTrigger("Patrolling Trigger");
            waitingTimer.Reset();
        }
        
        
    }
    public void CallAssistance()
    {
        if (!calledAssistance)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, callAssistanceRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.transform != null)
                {
                    EnemyAI otherEnemy = hitCollider.transform.GetComponent<EnemyAI>();
                    if (otherEnemy != null)
                    {
                        otherEnemy.calledAssistance = true;
                        otherEnemy.ProvokeEnemy();
                    }
                }
            }
            calledAssistance = true;
        }
    }
    public void ConfuseEnemyAfterAbduction()
    {
        Stopwatch timer = new Stopwatch();
        double randomMillis = new System.Random().Next(1000* (confusionDuration - 1), 1000 * (confusionDuration + 1));
        while(timer.ElapsedMilliseconds < randomMillis)
        {

        }
        timer.Reset();
    }
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 40);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 4);
    }

    public void ProvokeEnemy()
    {
        GetComponent<Animator>().SetTrigger("Provoked Trigger");
        isProvoked = true;
        waitingTimer.Reset();
    }
    public void CheckIfProvokedByRangeAndSpeed()
    {
        
        if (speed.magnitude > 7.8 && distanceToPlayer < chaseRangeMax - 6)
        {
            ProvokeEnemy();
        }
        else if (distanceToPlayer < chaseRangeMin)
        {
            ProvokeEnemy();
        }
    }

    public bool IfOtherEnemiesAreWithinProvokingRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, callAssistanceRadius - 5);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform != null)
            {
                EnemyAI otherEnemy = hitCollider.transform.GetComponent<EnemyAI>();
                if (otherEnemy != null && otherEnemy.GetDistanceToPlayer < chaseRangeMax && otherEnemy.isProvoked)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }
}
