using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public List<int> GainedTowerID = new();
    public Dictionary<int, int> TowerLevelDict = new();
    public int Gem = BasicGem;
    public int ReachedStage = BasicReachedStage;

    private const int BasicGem = 300;
    private const int BasicReachedStage = 1;
}
