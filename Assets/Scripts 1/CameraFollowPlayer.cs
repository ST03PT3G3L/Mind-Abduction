using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform pos;
    void Update()
    {
        transform.position = pos.position;
    }
}
