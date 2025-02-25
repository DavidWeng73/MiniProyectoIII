using UnityEngine;

public class CharacterControler : MonoBehaviour
{
    public Animator playerAnim;
    public CharacterController playerController;
    public Transform playerTrans;
    public float w_speed, wb_speed, olw_speed, rn_speed, ro_speed;
    public float jumpForce = 5f;
    public float gravity = 9.8f;
    private Vector3 moveDirection = Vector3.zero;
    private bool walking;

    void Update()
    {
        float moveZ = 0f;

        // Movimiento adelante/atrás
        if (Input.GetKey(KeyCode.W))
        {
            moveZ = w_speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveZ = -wb_speed;
        }

        Vector3 move = transform.forward * moveZ;

        // Manejo del salto
        if (playerController.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDirection.y = jumpForce;
                playerAnim.SetTrigger("Jump"); // Activar la animación de salto
                Debug.Log(" Salto activado en el Animator");
            }
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime; // Aplicar gravedad
        }

        playerController.Move((move + moveDirection) * Time.deltaTime);

        // Rotación con A y D
        if (Input.GetKey(KeyCode.A))
        {
            playerTrans.Rotate(0, -ro_speed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            playerTrans.Rotate(0, ro_speed * Time.deltaTime, 0);
        }

        // Animaciones de caminar
        if (Input.GetKeyDown(KeyCode.W))
        {
            playerAnim.SetTrigger("Walking");
            playerAnim.ResetTrigger("Idle");
            walking = true;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            playerAnim.ResetTrigger("Walking");
            playerAnim.SetTrigger("Idle");
            walking = false;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            playerAnim.SetTrigger("BackwardRunning");
            playerAnim.ResetTrigger("Idle");
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            playerAnim.ResetTrigger("BackwardRunning");
            playerAnim.SetTrigger("Idle");
        }

        // Correr
        if (walking)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                w_speed += rn_speed;
                playerAnim.SetTrigger("Running");
                playerAnim.ResetTrigger("Walking");
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                w_speed = olw_speed;
                playerAnim.ResetTrigger("Running");
                playerAnim.SetTrigger("Walking");
            }
        }
    }
}