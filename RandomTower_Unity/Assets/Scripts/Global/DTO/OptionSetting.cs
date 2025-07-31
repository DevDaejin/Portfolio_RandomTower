using System;
using System.Collections.Generic;

public class OptionSetting
{
    public Dictionary<string, Action<int>> LanguageDict = null;
    public Dictionary<string, Action<int>> ResolutionDict = null;
    public Dictionary<string, Action<int>> ScreenModeDict = null;
    public Action<int> BGMSliderCallback = null;
    public Action<int> SFXliderCallback = null;
    public Action ResetButtonCallback = null;
}
