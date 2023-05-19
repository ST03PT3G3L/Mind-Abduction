using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] public int damage = 15;
    [SerializeField] public float range = 50f;
    public float impactForce = 30;

    public Camera fpsCam;
    //public ParticleSystem muzzleFlash;
    //public GameObject impactEffect;
    public AudioSource beamSound;

    private void Start() {
        beamSound = GetComponent<AudioSource>();
 
         if (beamSound == null)
         {
             Debug.LogError("No AudioSource found");
         }
    }

    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }  

    void Shoot()
    {
        //muzzleFlash.Play();
        //beamSound.Play();

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            if (hit.transform != null)
            {
                EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
                if (target != null)
                {
                    target.ReceiveSomeDamage(damage, false);
                }
            }
        }

        //GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
       // Destroy(impactGO, 2f);  

        if (hit.rigidbody != null)
        {
            hit.rigidbody.AddForce(-hit.normal * impactForce);
        }
    }
}
