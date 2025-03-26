using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public int health = 3;
    public float freezeTime = 2f; 
    private bool isFrozen = false;
    private Rigidbody rb;
    public float UltedTime = 6f;
    private bool isUlted = false;
    private EnemyAI enemyAI; 
    private UnityEngine.AI.NavMeshAgent navAgent; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enemyAI = GetComponent<EnemyAI>();
        navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    public void TakeDamage(int damage)
    {
        if (!isFrozen)
        {
            health -= damage;
            Debug.Log("Enemigo golpeado, vida restante: " + health);

            StartCoroutine(UltEnemy());

            //if (health <= 0)
            //{
            //    Die();
            //}
        }
    }

    public void TakeUltDamage(int damage)
    {
        if (!isUlted)
        {
            health -= damage;
            Debug.Log("Enemigo golpeado, vida restante: " + health);

            StartCoroutine(FreezeEnemy());

            //if (health <= 0)
            //{
            //    Die();
            //}
        }
    }

    private IEnumerator FreezeEnemy()
    {
        isFrozen = true;
        if (navAgent != null)
        {
            navAgent.isStopped = true;
        }

        if (enemyAI != null)
        {
            enemyAI.enabled = false;
        }

        Debug.Log("Enemigo congelado por " + freezeTime + " segundos");

        yield return new WaitForSeconds(freezeTime);

        if (navAgent != null)
        {
            navAgent.isStopped = false;
        }

        if (enemyAI != null)
        {
            enemyAI.enabled = true;
        }

        isFrozen = false;
        Debug.Log("Enemigo descongelado");
    }

    private IEnumerator UltEnemy()
    {
        isUlted = true;
        if (navAgent != null)
        {
            navAgent.isStopped = true;
        }

        if (enemyAI != null)
        {
            enemyAI.enabled = false;
        }

        Debug.Log("Enemigo congelado por " + freezeTime + " segundos");

        yield return new WaitForSeconds(UltedTime);

        if (navAgent != null)
        {
            navAgent.isStopped = false;
        }

        if (enemyAI != null)
        {
            enemyAI.enabled = true;
        }

        isUlted = false;
        Debug.Log("Enemigo descongelado");
    }



    private void Die()
    {
        Debug.Log("Enemigo eliminado");
        Destroy(gameObject);
    }
}

