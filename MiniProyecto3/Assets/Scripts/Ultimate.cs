using UnityEngine;

public class Ultimate : MonoBehaviour
{
    public int damage = 3;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeUltDamage(damage);
            }
        }
    }
}
