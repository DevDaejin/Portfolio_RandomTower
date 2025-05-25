using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] private GlobalUI _global;
    [SerializeField] private GameObject _multiPanel;

    [Header("Menu")]
    [SerializeField] private Button _singleButton;
    [SerializeField] private Button _multiButton;
    [SerializeField] private Button _optionButton;
    [SerializeField] private Button _exitButton;

    [Header("URI")]
    [SerializeField] private GameObject _uriPanel;
    [SerializeField] private TMP_InputField _ipInputField;
    [SerializeField] private TMP_Text ipFeedbackTxt;
    [SerializeField] private TMP_InputField _portInputField;
    [SerializeField] private Button _multiConfirmButton;
    [SerializeField] private Button _multiCancelButton;

    public event Action<string> OnIpInput;
    public event Action<string> OnPortInput;
    public event Action<string ,string> OnMutliConfirm;

    [Header("Lobby")]
    [SerializeField] private GameObject _lobbyPanel;
    [SerializeField] private GameObject _roomButton;
    [SerializeField] private Button _lobbyCancelButton;

    private void Start()
    {
        _singleButton.onClick.AddListener(GoToLobby);

        _multiButton.onClick.AddListener(() =>
        {
            _multiPanel.SetActive(true);
            _uriPanel.SetActive(true);
            _lobbyPanel.SetActive(false);
        });
        
        _ipInputField.onEndEdit.AddListener(EndEditIP);
        _portInputField.onEndEdit.AddListener(EndEditPort);

        _multiConfirmButton.onClick.AddListener(MultiConfirm);
        _multiCancelButton.onClick.AddListener(MultiCancel);

        _global.SetQuitConfrimButton(Application.Quit);
        _global.SetQuitCancelButton(() => _global.gameObject.SetActive(false));

        _optionButton.onClick.AddListener(() => _global.Set(GlobalUI.GlobalUIOption.Option));
        _exitButton.onClick.AddListener(() => _global.Set(GlobalUI.GlobalUIOption.Quit));
    }

    private void EndEditIP(string ip)
    {
        if(IsValidIP(ip))
        {
            ipFeedbackTxt.text = string.Empty;
            OnIpInput?.Invoke(ip);
        }
        else
        {
            ipFeedbackTxt.text = "IP 형식이 올바르지 않습니다.";
        }
    }

    private void EndEditPort(string port)
    {
        if(string.IsNullOrEmpty(port)) return;

        OnPortInput?.Invoke(port);
    }

    private bool IsValidIP(string ip)
    {
        return Regex.IsMatch(ip, @"^((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)\.){3}(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)$");
    }

    private void MultiConfirm()
    {
        string ip = string.Empty;
        string port = string.Empty; 

        if (ipFeedbackTxt.text != string.Empty ||
            string.IsNullOrEmpty(_ipInputField.text) ||
            string.IsNullOrEmpty(_portInputField.text))
        {
            return;
        }

        _uriPanel.SetActive(false);
        _lobbyPanel.SetActive(true);
        OnMutliConfirm?.Invoke(_ipInputField.text, _portInputField.text);
    }

    private void MultiCancel()
    {
        _ipInputField.text = string.Empty;
        _portInputField.text = string.Empty;
        ipFeedbackTxt.text = string.Empty;
        _multiPanel.SetActive(false);
    }

    private void GoToLobby()
    {
        GameManager.Instance.LoadScene(GameManager.Scenes.Lobby);
    }
}
