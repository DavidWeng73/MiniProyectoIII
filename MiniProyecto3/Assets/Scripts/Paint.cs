using UnityEngine;
using System.Collections;

public class Paint : MonoBehaviour
{
    private Rigidbody rb;
    public static int numPaints = 0;
    public GameObject puerta;

    private void Start()
    {
        if (numPaints >= 3) numPaints = 0;
    }
    public void DestroyPaint()
    {
        Debug.Log("Cuadro Completado");
        numPaints++;

        if (numPaints == 3 && puerta != null)
        {
            puerta.SetActive(false); 
        }

        Destroy(gameObject);
    }
}
