using UnityEngine;
using TMPro;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text relayCodeText;

    private void Start()
    {
        if (RelayManager.Instance == null)
        {
            relayCodeText.text = "Error: no RelayManager";
            return;
        }

        string code = RelayManager.Instance.JoinCode;

        if (!string.IsNullOrEmpty(code))
            relayCodeText.text = $"{code}";
        else
            relayCodeText.text = "Esperando código de unión...";
    }
}
