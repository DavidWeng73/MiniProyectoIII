using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 1;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        if (other.CompareTag("Paints"))
        {
            Paint paint = other.GetComponent<Paint>();
            if (paint != null)
            {
                paint.DestroyPaint();
            }
        }

        if (other.CompareTag("FakePaints"))
        {
            Paint fakepaint = other.GetComponent<Paint>();
            if (fakepaint != null)
            {
                fakepaint.DestroyPaint();
            }

        }
    }
}