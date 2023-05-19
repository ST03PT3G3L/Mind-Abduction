using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class AIEnemy : MonoBehaviour
{
    [SerializeField] public Transform target;
    [SerializeField] float chaseRange = 5f;
    [SerializeField] float turnSpeed = 5f;

    UnityEngine.AI.NavMeshAgent navMeshAgent;
    [SerializeField] float distanceToTarget = Mathf.Infinity;
    [SerializeField] bool isProvoked = false;

    [SerializeField] bool confused = false;

    void Start()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    void OnEnable()
    {
        gameObject.tag = "Enemy";
        gameObject.GetComponent<PlayerMovement2>().enabled = false;
        gameObject.GetComponent<Sliding>().enabled = false;
        gameObject.GetComponentInChildren<Camera>().enabled = false;
        gameObject.GetComponent<MindAbduction>().enabled = false;
        gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        gameObject.GetComponent<CapsuleCollider>().enabled = true;

        Targetting();
    }

    private void Targetting()
    {
        gameObject.GetComponentInParent<EnemyController>().ResetTarget();
    }

    public void SetTarget()
    {
        Debug.Log("Hello");
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Confusion()
    {
        confused = true;
        StartCoroutine(NoMoreConfuse());
    }

    IEnumerator NoMoreConfuse()
    {
        yield return new WaitForSeconds(5);
        confused = false;
    }

    void Update()
    {
        if (!confused)
        {
            distanceToTarget = Vector3.Distance(target.position, transform.position);

            if (isProvoked)
            {
                EngageTarget();
            }
            else if (distanceToTarget <= chaseRange)
            {
                isProvoked = true;
            }
        }
    }

    private void EngageTarget()
    {
        navMeshAgent.SetDestination(target.position);
    }
}
