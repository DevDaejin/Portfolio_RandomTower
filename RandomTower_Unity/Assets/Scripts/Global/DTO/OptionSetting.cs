using System;
using System.Collections.Generic;

public class OptionSetting
{
    public Dictionary<string, Action> LanguageDict;
    public Dictionary<string, Action> ResolutionDict;
    public Dictionary<string, Action> ScreenModeDict;
    public Action<float> BGMSliderCallback;
    public Action<float> SFXliderCallback;
    public Action ResetButtonCallback;
}
