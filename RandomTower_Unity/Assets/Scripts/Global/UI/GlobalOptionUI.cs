using System;
using System.Collections.Generic;
using TMPro;
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

    [SerializeField] private Button _closeButton;

    private Action _onClose;

    private TMP_Text _bgmValueTxt;
    private TMP_Text _sfxValueTxt;

    private OptionSetting _setting;

    public void Initialize(OptionSetting setting, OptionSaveData saveData, Action onClose)
    {
        _setting = setting;
        _onClose = onClose;

        BindDropdown(_languageDropdown, _setting.LanguageDict);
        BindDropdown(_resolutionDropdown, _setting.ResolutionDict);
        BindDropdown(_screenModeDropdown, _setting.ScreenModeDict);
        _screenModeDropdown.GetComponent<LocalizedDropdown>().Initialzie();

        BindSlider(_bgmSlider, _bgmValueTxt, _setting.BGMSliderCallback);
        BindSlider(_sfxSlider, _sfxValueTxt, _setting.SFXliderCallback);

        InitializeResetButton();

        _closeButton.onClick.RemoveAllListeners();
        _closeButton.onClick.AddListener(() =>
        {
            ActiveUI(false);
            _onClose?.Invoke();
        });

        _languageDropdown.value = saveData.LanguageCode;
        _screenModeDropdown.value = saveData.ScreenModeCode;
        _resolutionDropdown.value = saveData.ResolutionCode;

        _bgmSlider.value = saveData.BGM;
        _sfxSlider.value = saveData.SFX;
    }

    private void BindDropdown(TMP_Dropdown dropdown, Dictionary<string, Action<int>> options)
    {
        dropdown.ClearOptions();

        var keys = new List<string>(options.Keys);
        dropdown.AddOptions(keys);

        dropdown.onValueChanged.RemoveAllListeners();
        dropdown.onValueChanged.AddListener(index =>
        {
            if (options.TryGetValue(keys[index], out var action))
            {
                action?.Invoke(index);
            }
        });

        ResizeDropdownTemplate(dropdown);
    }

    private void BindSlider(Slider slider, TMP_Text valueTxt, Action<int> callback)
    {
        slider.onValueChanged.RemoveAllListeners();

        valueTxt ??= slider.GetComponentInChildren<TMP_Text>();

        slider.onValueChanged.AddListener((value) =>
        {
            valueTxt.text = (value).ToString();
            callback?.Invoke((int)value);
        });
    }

    private void InitializeResetButton()
    {
        _resetDataButton.onClick.RemoveAllListeners();
        _resetDataButton.onClick.AddListener(OnResetButton);
    }

    private void OnResetButton() => GameManager.Instance.UI.Global.ShowReset(_setting.ResetButtonCallback);

    private void ResizeDropdownTemplate(TMP_Dropdown dropdown)
    {
        var content = dropdown.template.Find("Viewport/Content")?.GetComponent<RectTransform>();
        var item = dropdown.template.GetComponentInChildren<Toggle>(true);
        if (content == null || item == null) return;

        float itemHeight = item.GetComponent<RectTransform>().rect.height;
        float spacing = content.GetComponent<VerticalLayoutGroup>()?.spacing ?? 0f;
        float padding = content.GetComponent<VerticalLayoutGroup>()?.padding.vertical ?? 0f;

        int optionCount = dropdown.options.Count;
        float totalHeight = (itemHeight + spacing) * optionCount + padding;
        float maxHeight = 300f;

        float finalHeight = Mathf.Min(totalHeight, maxHeight);
        dropdown.template.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, finalHeight);
    }

    public void ActiveUI(bool isAct, bool includeReset = false)
    {
        _optionPanel.SetActive(isAct);
        _resetDataButton.interactable = includeReset;
    }
}
