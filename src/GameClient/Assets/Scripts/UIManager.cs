using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField]
    private GameObject _startMenu;

    [SerializeField]
    private InputField _userNameField;

    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void ConnectToServer()
    {
        _startMenu.SetActive(false);
        _userNameField.interactable = false;

        Client.Instance.ConnectToServer();
    }
}
