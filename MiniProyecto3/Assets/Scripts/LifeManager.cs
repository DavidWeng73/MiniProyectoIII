using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LifeManager : NetworkBehaviour
{
    public static LifeManager Instance;
    public NetworkVariable<int> currentLives = new NetworkVariable<int>(5, NetworkVariableReadPermission.Everyone);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        currentLives.OnValueChanged += OnLifeChanged;

        OnLifeChanged(0, currentLives.Value);
    }

    public void PlayerLostLife()
    {
        if (!IsServer) return;
        if (IsServer && currentLives.Value > 0)
        {
            currentLives.Value--;
        }
    }

    private void OnLifeChanged(int oldVal, int newVal)
    {
        Debug.Log($"[SharedLifeManager] Vidas actualizadas: {newVal}");

        LifeUI ui = Object.FindFirstObjectByType<LifeUI>();
        if (ui != null)
            ui.UpdateLives(newVal);

        if (newVal <= 0)
        {
            Debug.Log("[SharedLifeManager] ¡Juego perdido!");
            NetworkManager.SceneManager.LoadScene("LoseScene", LoadSceneMode.Single);
        }
    }
}

