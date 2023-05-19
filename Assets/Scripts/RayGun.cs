using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayGun : MonoBehaviour
{
    [SerializeField] Camera mainCam;
    public float shootRate;
    private float m_ShootRateTimeStamp;
    
    public GameObject shootPrefab;
    RaycastHit hit;
    float range = 1000f;

    void Update()
    {

        if (Input.GetButtonDown("Fire1"))
        {
            if (Time.time > m_ShootRateTimeStamp)
            {
                ShootRay(mainCam);
                m_ShootRateTimeStamp = Time.time + shootRate;
            }
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
        }
    }
}

