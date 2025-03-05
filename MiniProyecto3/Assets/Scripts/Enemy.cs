using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    public int health = 3;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Enemigo golpeado, vida restante: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()  
    {
        Debug.Log("Enemigo eliminado");
        Destroy(gameObject);
    }
}

