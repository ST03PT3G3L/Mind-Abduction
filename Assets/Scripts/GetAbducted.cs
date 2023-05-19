using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class GetAbducted : MonoBehaviour
{
    [SerializeField] GameObject weapon;
    [SerializeField] GameObject Muzzle;
    [SerializeField] Camera camera;

    public void Abducted()
    {
        gameObject.tag = "Player";
        gameObject.GetComponent<PlayerMovement2>().enabled = true;
        gameObject.GetComponent<Sliding>().enabled = true;
        camera.GetComponent<Camera>().enabled = true;
        camera.GetComponent<PlayerLook>().enabled = true;
        gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        gameObject.GetComponent<MindAbduction>().enabled = true;
        gameObject.GetComponent<EnemyAI>().enabled = false;
        gameObject.GetComponent<CapsuleCollider>().enabled = true;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<EnemyAttack>().enabled = false;
        gameObject.GetComponent<WaypointMover>().enabled = false;
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        weapon.GetComponent<Weapon>().enabled = true;
        Muzzle.GetComponent<RayGun>().enabled = true;
        gameObject.GetComponent<EnemyAttack>().enabled = false;
    }
}
