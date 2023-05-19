using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MindAbduction : MonoBehaviour
{
    [SerializeField] float range = 5f;
    [SerializeField] Camera FPCamera;

    [SerializeField] GameObject controller;

    [SerializeField] GameObject Energy; 

    public void OnEnable()
    {
        FPCamera = this.GetComponentInChildren<Camera>();
        controller = GameObject.FindGameObjectWithTag("Controller");
    }

    public void Update()
    {
        if(Input.GetKeyDown("e"))
        {
                Abduct();
        }
    }

    private void Abduct()
    {
        RaycastHit hit;
        if(Physics.Raycast(FPCamera.transform.position, FPCamera.transform.forward, out hit, range))
        {
            GetAbducted target = hit.transform.GetComponent<GetAbducted>();
            Debug.Log("You hit: " + target);
            if(target == null) return;
            if (target.tag == "Enemy")
            {
                target.Abducted();


                if (gameObject.name == "PlayerEnergy" || gameObject.name == "PlayerEnergy(Clone)")
                {
                    Destroy(gameObject);
                }
                else
                {
                    try
                    {
                        gameObject.GetComponent<EnemyAI>().enabled = true;
                        //gameObject.GetComponent<EnemyAI>().Confusion();
                    }
                    catch
                    {
                        Debug.Log("EnemyAI does not exist");
                    }
                }
            }
        }
        else
        {
            if(gameObject.name != "PlayerEnergy" || gameObject.name != "PlayerEnergy(Clone)")
            {
                gameObject.GetComponent<EnemyAI>().enabled = true;
                //gameObject.GetComponent<AIEnemy>().Confusion();

                Vector3 pos = new Vector3(transform.position.x, transform.position.y+10f, transform.position.z);
                GameObject Player = Instantiate(Energy, pos, transform.rotation);
            }
        }
    }
}
