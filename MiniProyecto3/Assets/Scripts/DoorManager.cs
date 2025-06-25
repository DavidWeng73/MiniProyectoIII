using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class DoorManager : NetworkBehaviour
{
    public string nextLevel;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer || !other.CompareTag("Player")) return;
        NetworkManager.Singleton.SceneManager.LoadScene(nextLevel, LoadSceneMode.Single);
    }
}