using UnityEngine;

public class FootStep : MonoBehaviour
{
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private SoundsVariety[] sounds;
    [SerializeField] private float walkStepRate = 0.5f;
    [SerializeField] private float runStepRate = 0.3f;
    [SerializeField] private CharacterController characterController;

    private float stepTimer = 0f;

    private void Update()
    {
        bool isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        bool isGrounded = characterController.isGrounded;
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float currentStepRate = isRunning ? runStepRate : walkStepRate;

        if (isMoving && isGrounded)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootStep();
                stepTimer = currentStepRate;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    private void PlayFootStep()
    {
        int index = Random.Range(0, sounds.Length);
        SoundsVariety selected = sounds[index];

        if (selected.randomPitch)
        {
            m_AudioSource.pitch = Random.Range(selected.pitch - 0.3f, selected.pitch + 0.8f);
        }
        else
        {
            m_AudioSource.pitch = selected.pitch;
        }

        m_AudioSource.PlayOneShot(selected.clip);
    }
}
