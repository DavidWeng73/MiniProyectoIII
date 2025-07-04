using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class DoorManager : NetworkBehaviour
{
    [SerializeField] public string nextLevel;
    [SerializeField] private string coopScene = "NivelCoop";
    private bool hasTriggered = false;
    private float activationDelay = 2f; 
    private float sceneLoadTime;

    private void Start()
    {
        sceneLoadTime = Time.time;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer || hasTriggered) return;

        if (Time.time - sceneLoadTime < activationDelay) return; 

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            string sceneToLoad = NetworkManager.Singleton.ConnectedClients.Count > 1 ? coopScene : nextLevel;
            NetworkManager.Singleton.SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
        }
    }
}
