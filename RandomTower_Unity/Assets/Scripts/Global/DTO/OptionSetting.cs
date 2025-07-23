using System;
using System.Collections.Generic;

public class OptionSetting
{
    public Dictionary<string, Action> LanguageDict = null;
    public Dictionary<string, Action> ResolutionDict = null;
    public Dictionary<string, Action> ScreenModeDict = null;
    public Action<float> BGMSliderCallback = null;
    public Action<float> SFXliderCallback = null;
    public Action ResetButtonCallback = null;
}
