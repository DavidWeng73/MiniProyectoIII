using UnityEngine;
using System.Collections;

public class Paint : MonoBehaviour
{
    public static int numPaints = 0;
    public GameObject puerta;
    public GameObject player;

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

    public void FakePaintTrap()
    {
        StartCoroutine(FakePaintTrapCoroutine());
    }

    private IEnumerator FakePaintTrapCoroutine()
    {
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            yield return new WaitForSeconds(5f);
            controller.enabled = true;
        }
    }
}
