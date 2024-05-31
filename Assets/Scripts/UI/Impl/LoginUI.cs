using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUIHandler : UI
{
    [SerializeField] private TMP_InputField _usernameInput;
    [SerializeField] private Button _connectButton;

    void HandleLogin()
    {
        string username = _usernameInput.text;

        NetworkManager networkManager = NetworkManager.Instance;
        networkManager.Token = username;
        networkManager.Connect();
    }

    void Start()
    {
        _connectButton.onClick.RemoveAllListeners();
        _connectButton.onClick.AddListener(HandleLogin);
    }
}
