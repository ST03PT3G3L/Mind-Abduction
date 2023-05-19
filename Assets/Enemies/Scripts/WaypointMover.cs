using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WaypointMover : MonoBehaviour
{
    [SerializeField] Waypoints waypoints;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float moveLimit = 0.1f;
    Transform way;
    // Start is called before the first frame update
    void Start()
    {
        way = waypoints.GetNextWaypoint(way);
        transform.position = way.position;

        way = waypoints.GetNextWaypoint(way);
    }

    // Update is called once per frame
    void Update()
    {
        bool isProvoked = transform.GetComponent<EnemyAI>().isProvoked;
        if (!isProvoked)
        {
            transform.position = Vector3.MoveTowards(transform.position, way.position, moveSpeed * Time.deltaTime);
            Vector3 direction = (way.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            if (Vector3.Distance(transform.position, way.position) < 1.5f)
            {
                way = waypoints.GetNextWaypoint(way);
            }
        }
    }
}
