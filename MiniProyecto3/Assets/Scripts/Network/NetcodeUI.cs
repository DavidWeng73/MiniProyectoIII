using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;

public class NetcodeUI : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;
    [SerializeField] private TMP_InputField joinCodeInput;

    private RelayManager relayManager;

    private void Awake()
    {
        relayManager = Object.FindFirstObjectByType<RelayManager>();

        if (relayManager == null)
        {
            Debug.LogError("RelayManager no encontrado en la escena.");
            return;
        }

        startHostButton.onClick.AddListener(() =>
        {
            Debug.Log("HOST (Relay)");
            relayManager.CreateRelay();
        });

        startClientButton.onClick.AddListener(() =>
        {
            Debug.Log("CLIENT (Relay)");
            if (!string.IsNullOrEmpty(joinCodeInput.text))
            {
                relayManager.JoinRelay(joinCodeInput.text);
            }
            else
            {
                Debug.LogWarning("No se ha introducido código de unión");
            }
        });
    }
}
