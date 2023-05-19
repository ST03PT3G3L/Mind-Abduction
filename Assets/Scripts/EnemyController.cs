using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private void Start()
    {
        BroadcastMessage("SetTarget");
    }

    public void ResetTarget()
    {
        //BroadcastMessage("SetTarget");
    }

    private void Update() {
        BroadcastMessage("SetTarget");
    }
}
