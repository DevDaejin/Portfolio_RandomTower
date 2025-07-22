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

    private TMP_Text _bgmValueTxt;
    private TMP_Text _sfxValueTxt;

    private OptionSetting _data;

    public void Initialize(OptionSetting data)
    {
        _data = data;

        BindDropdown(_languageDropdown, _data.LanguageDict);
        BindDropdown(_resolutionDropdown, _data.ResolutionDict);
        BindDropdown(_screenModeDropdown, _data.ScreenModeDict);

        BindSlider(_bgmSlider, _bgmValueTxt, _data.BGMSliderCallback);
        BindSlider(_sfxSlider, _sfxValueTxt, _data.SFXliderCallback);
        
        InitializeResetButton();

        _closeButton.onClick.RemoveAllListeners();
        _closeButton.onClick.AddListener(() => ActiveUI(false));
    }

    private void BindDropdown(TMP_Dropdown dropdown, Dictionary<string, Action> options)
    {
        dropdown.ClearOptions();

        var keys = new List<string>(options.Keys);
        dropdown.AddOptions(keys);

        dropdown.onValueChanged.RemoveAllListeners();
        dropdown.onValueChanged.AddListener(index =>
        {
            if (options.TryGetValue(keys[index], out var action))
            {
                action?.Invoke();
            }
        });

        ResizeDropdownTemplate(dropdown);
    }

    private void BindSlider(Slider slider, TMP_Text valueTxt, Action<float> callback)
    {
        slider.onValueChanged.RemoveAllListeners();

        valueTxt ??= slider.GetComponentInChildren<TMP_Text>();

        slider.onValueChanged.AddListener((value)=>
        {
            valueTxt.text = (value).ToString();
            callback?.Invoke(value / slider.maxValue);
        });
    }

    private void InitializeResetButton()
    {
        _resetDataButton.onClick.RemoveAllListeners();
        _resetDataButton.onClick.AddListener(OnResetButton);
    }

    private void OnResetButton() => GameManager.Instance.UI.Global.ShowReset(_data.ResetButtonCallback);

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
        _resetDataButton.gameObject.SetActive(includeReset);
    }
}
