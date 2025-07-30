using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class OptionManager : MonoBehaviour
{
    private Coroutine _localeRoutine = null;
    private Dictionary<string, string> _localeDict = new()
    {
        {"ko-KR", "한국어" }, {"en", "English"}
    };

    private Vector2Int[] _resolutionArray = new Vector2Int[]
    {
        new Vector2Int(1280, 720),
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440),
        new Vector2Int(3840, 2160),
    };

    private Dictionary<FullScreenMode, string> _screenModeDict = new()
    {
        { FullScreenMode.ExclusiveFullScreen, "전체 화면"},
        { FullScreenMode.FullScreenWindow, "테두리 없음"},
        { FullScreenMode.MaximizedWindow, "윈도우"}
    };


    public void Initialize(Action<float> OnChagnedBGMVolume, Action<float> OnChagnedSFXVolume)
    {
        GlobalUI globalUI = GameManager.Instance.UI.Global;
        LocalDataManager dataManager = GameManager.Instance.Data;

        var optionData = new OptionSetting
        {
            LanguageDict = InitializeLanguage(),
            ResolutionDict = InitializeResolution(),
            ScreenModeDict = InitializeScreenMode(),

            BGMSliderCallback = OnChagnedBGMVolume,
            SFXliderCallback = OnChagnedSFXVolume,

            ResetButtonCallback = dataManager.Reset
        };

        globalUI.Initialize(optionData);
    }

    private Dictionary<string, Action> InitializeLanguage()
    {
        Dictionary<string, Action> dict = new();

        foreach (var codeLabel in _localeDict)
        {
            dict.Add(codeLabel.Value, () => ChangeLocale(codeLabel.Key));
        }

        return dict;
    }

    private Dictionary<string, Action> InitializeResolution()
    {
        Dictionary<string, Action> dict = new();

        foreach (var resolution in _resolutionArray)
        {
            dict.Add(ResolutionVector2ToString(resolution), () => SetResolution(resolution));
        }

        return dict;
    }

    private Dictionary<string, Action> InitializeScreenMode()
    {
        Dictionary<string, Action> dict = new();

        foreach (var mode in _screenModeDict)
        {
            dict.Add(mode.Value, () => SetFullscreen(mode.Key));
        }

        return dict;
    }

    private void ChangeLocale(string code)
    {
        if (_localeRoutine != null)
        {
            StopCoroutine(_localeRoutine);
            _localeRoutine = null;
        }

        _localeRoutine = StartCoroutine(ChangeLocaleRoutine(code));
    }

    private IEnumerator ChangeLocaleRoutine(string code)
    {
        yield return LocalizationSettings.InitializationOperation;

        Locale selected = null;

        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            Debug.Log(code);
            if (locale.Identifier.Code == code)
            {
                selected = locale;
                break;
            }
        }

        LocalizationSettings.SelectedLocale = selected;
    }

    private string ResolutionVector2ToString(Vector2Int resolution) => $"{resolution.x} X {resolution.y}";

    private void SetResolution(Vector2Int resolution)
    {
        var screenMode = Screen.fullScreen;
        Screen.SetResolution(resolution.x, resolution.y, screenMode);
    }

    private void SetFullscreen(FullScreenMode mode) => Screen.fullScreenMode = mode;
}
