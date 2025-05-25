using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalUI : MonoBehaviour
{
    public enum GlobalUIOption { None, Option, Quit };

    [Header("Option")]
    [SerializeField] private GameObject _optionPanel;
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private Button _optionConfirmButton;
    [SerializeField] private Button _optionCancelButton;

    [Header("Quit")]
    [SerializeField] private GameObject _quitPanel;
    [SerializeField] private Button _quitConfirmButton;
    [SerializeField] private Button _quitCancelButton;

    private void Start()
    {
        _optionConfirmButton.onClick.AddListener(OnOptionConfirm);
        
        _optionCancelButton.onClick.AddListener(OnOptionCancel);
    }

    public void Set(GlobalUIOption option)
    {
        gameObject.SetActive(true);

        _optionPanel.SetActive(option == GlobalUIOption.Option);
        _quitPanel.SetActive(option == GlobalUIOption.Quit);
    }

    private void OnOptionConfirm()
    {
        //TODO: 적용 로직
        gameObject.SetActive(false);
    }

    private void OnOptionCancel()
    {
        //TODO: 원복 로직
        gameObject.SetActive(false);
    }


    public void SetQuitConfrimButton(Action callback)
    {
        _quitConfirmButton.onClick.AddListener(callback.Invoke);
    }

    public void SetQuitCancelButton(Action callback)
    {
        _quitCancelButton.onClick.AddListener(callback.Invoke);
    }
}
