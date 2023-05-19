using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;

public class EnemyHealth : MonoBehaviour
{
    private EnemyAI enemyAi;
    [SerializeField] int enemyHp = 100;
    [SerializeField] float ArmourBlockPercentage;
    [SerializeField] int armourHp;
    [SerializeField] GameObject Energy;

    private void Start()
    {
        enemyAi = GetComponent<EnemyAI>();
    }

    public bool ReceiveSomeDamage(int receivedDamage, bool isHeadshot)
    {
        TriggerOnReceivedDmg();
        if (armourHp >= receivedDamage) //if armour is still alive then let it tank damage
        {
            armourHp -= receivedDamage;
            UnityEngine.Debug.Log("Blocked " + receivedDamage + " hp, " + armourHp + " armor is left and " + enemyHp + " is left" );

            return false;
        }
        else 
        {
            if (armourHp > 0) //if it wouldn't last, calculate the damage that the armour didn't block
            {
                int hpLeftToDeduct = receivedDamage - armourHp;
                armourHp = 0;
                enemyHp -= hpLeftToDeduct; //and deduct it from the enemy
                UnityEngine.Debug.Log("Armour down! " + hpLeftToDeduct + " hp dealt, " + enemyHp + " is left");
            }
            else //if the armour is already dead, deduct the passive damage block, check for headshots and then calculate enemys health
            {

                enemyHp -= AdditionalDamageCalcs(isHeadshot, receivedDamage);
                UnityEngine.Debug.Log(receivedDamage + " damage dealt, " + enemyHp + " is left");
            }
            if (enemyHp <= 0) //if no hp is left the enemy is dead
            {
                if(gameObject.tag == "Player")
                {
                    Vector3 pos = new Vector3(transform.position.x, transform.position.y + 10f, transform.position.z);
                    GameObject Player = Instantiate(Energy, pos, transform.rotation);
                }
                UnityEngine.Debug.Log("Enemy was killed!");
                Destroy(gameObject);
                return true;
            }
            return false; 
        }

    }

    public void TriggerOnReceivedDmg() //when enemy got shot, it would be provoked after defined delay
    {
            enemyAi.ProvokeEnemy();
    }


    public int AdditionalDamageCalcs(bool isHeadshot, double receivedDamage) //here we calculate passive boost from armor and headshots
    {
        if (isHeadshot)
        {
            receivedDamage *= 2.5;
        }
        double finalDamageDealt = ((100 - ArmourBlockPercentage) / 100) * receivedDamage;
        return Convert.ToInt32(Math.Round(finalDamageDealt));
    }
    
    
}
