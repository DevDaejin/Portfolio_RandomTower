using System;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
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
    [SerializeField] private Button _uriConfirmButton;
    [SerializeField] private Button _uriCancelButton;

    public Action SinglePlayButton;
    public Action QuitButton;

    public event Action<string> OnIpInput;
    public event Action<string> OnPortInput;
    public event Action<string, string> OnMultiConfirm;

    [Header("Connecting")]
    [SerializeField] private GameObject _connectingPanel;
    [SerializeField] private Button _connectingCancelButton;
    [SerializeField] private TMP_Text _connectingTxt;
    private readonly StringBuilder _connectBuilder = new(); 
    public event Action OnConnectingCancel;

    private float _dotElpased = 0;
    private float _dotDuration = 0.5f;
    private int _dotIndex = 0;
    private const int _dotMax = 4;
    private const string ConnectiText = "Connecting";
    private const string IPFeedback = "IP 형식이 올바르지 않습니다.";

    private void Start()
    {
        _singleButton.onClick.AddListener(SinglePlayButton.Invoke);

        _multiButton.onClick.AddListener(() =>
        {
            _uriPanel.SetActive(true);
            _connectingPanel.SetActive(false);
        });

        _ipInputField.onEndEdit.AddListener(EndEditIP);
        _portInputField.onEndEdit.AddListener(EndEditPort);

        _uriConfirmButton.onClick.AddListener(MultiConfirm);
        _uriCancelButton.onClick.AddListener(MultiCancel);

        _connectingCancelButton.onClick.AddListener(ConnectingCancel);


        GlobalUI global = GameManager.Instance.UI.Global;
        global.SetQuitConfrimButton(QuitButton.Invoke);
        global.SetQuitCancelButton(() => global.gameObject.SetActive(false));

        _optionButton.onClick.AddListener(() => global.Set(GlobalUI.GlobalUIOption.Option));
        _exitButton.onClick.AddListener(() => global.Set(GlobalUI.GlobalUIOption.Quit));
    }

    private void Update()
    {
        if (_connectingPanel.activeInHierarchy)
        {
            _dotElpased += Time.deltaTime;

            if (_dotElpased < _dotDuration) return;

            _dotElpased = 0;

            _connectBuilder.Clear();
            _connectBuilder.Append(ConnectiText).Append('.', _dotIndex);
            _connectingTxt.text = _connectBuilder.ToString();

            _dotIndex++;
            if (_dotIndex >= _dotMax) _dotIndex = 0;
        }
    }

    private void EndEditIP(string ip)
    {
        if (IsValidIP(ip))
        {
            ipFeedbackTxt.text = string.Empty;
            OnIpInput?.Invoke(ip);
        }
        else
        {
            ipFeedbackTxt.text = IPFeedback;
        }
    }

    private void EndEditPort(string port)
    {
        if (string.IsNullOrEmpty(port)) return;

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
        _connectingPanel.SetActive(true);
        OnMultiConfirm?.Invoke(_ipInputField.text, _portInputField.text);
    }

    private void MultiCancel()
    {
        _ipInputField.text = string.Empty;
        _portInputField.text = string.Empty;
        ipFeedbackTxt.text = string.Empty;
        _uriPanel.SetActive(false);
    }

    private void ConnectingCancel()
    {
        OnConnectingCancel?.Invoke();
        _connectingPanel.SetActive(false);
        _uriPanel.SetActive(true);
    }
}
