using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] int enemyDamage = 25;
    [SerializeField] int enemyShootingFrequency = 1;
    // Start is called before the first frame update

    [SerializeField] public float range = 50f;
    public float impactForce = 30;
    public bool couldShoot = false;
    public Camera fpsCam;
    public GameObject shootPrefab;
    RaycastHit hit;
    //public ParticleSystem muzzleFlash;
    //public GameObject impactEffect;
    Stopwatch shootingFrequency;
    void Start()
    {
        shootingFrequency = new Stopwatch();
        shootingFrequency.Start();
    }
    void Update()
    {
        int millis = new System.Random().Next(750, 1550); 
        if (!shootingFrequency.IsRunning && couldShoot)
        {
            shootingFrequency.Start();
        }

        if (shootingFrequency.ElapsedMilliseconds > enemyShootingFrequency * millis)
        {
            Shoot();
            ShootRay(fpsCam);
            shootingFrequency.Reset();
        }
    }

    void Shoot()
    {
            //muzzleFlash.Play();
            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
            {
                UnityEngine.Debug.Log(hit.transform.name);
                if (hit.transform != null)
                {
                }
            }

            //GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            // Destroy(impactGO, 2f);  

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }
    }

    public void ShootRay(Camera camera)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, range))
        {
            GameObject laser = GameObject.Instantiate(shootPrefab, transform.position, transform.rotation) as GameObject;
            laser.GetComponent<ShotBehavior>().setTarget(hit.point);
            GameObject.Destroy(laser, 2f);
            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
            if (target.tag == "Player")
            {
                target.ReceiveSomeDamage(enemyDamage, false);
            }
        }
    }
}
