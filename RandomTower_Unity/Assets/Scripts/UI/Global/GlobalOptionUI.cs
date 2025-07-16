using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.UI;

public class GlobalOptionUI : MonoBehaviour
{
    public GameObject Panel => _optionPanel;
    [SerializeField] private GameObject _optionPanel;

    [SerializeField] private TMP_Dropdown _languageDropdown;
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private TMP_Dropdown _screenModeDropdown;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Button _resetDataButton;

    public void InitializeLanguage(
        Dictionary<string, Action> languageDict,
        Dictionary<string, Action> resolutionDict,
        Dictionary<string, Action> screenModeDict,
        Action bgmSliderCallback,
        Action sfxSliderCallback,
        Action resetButtonCallback)
    {
        
    }

    public void ActiveUI(bool isAct) => _optionPanel.SetActive(isAct);
}
