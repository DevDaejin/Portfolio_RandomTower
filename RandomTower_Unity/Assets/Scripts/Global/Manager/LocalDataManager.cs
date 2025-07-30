using System;
using System.Collections.Generic;
using UnityEngine;


public class LocalDataManager
{
    private TowerDatabase _towerDB;

    public LocalDataManager(TowerDatabase towerDB)
    {
        _towerDB = towerDB;
        Load();
    }

    [Serializable]
    public class SaveData
    {
        public List<int> GainedTowerID = new();
        public Dictionary<int, int> TowerLevelDict = new();
        public int Gem = BasicGem;
        public int ReachedStage = BasicReachedStage;

        public OptionSaveData Option = new();
    }

    public SaveData Loaded { get; private set; }

    private const int BasicGem = 300;
    private const int BasicReachedStage = 1;
    private const string Key = "LocalProgress";

    public void Load()
    {
        if (PlayerPrefs.HasKey(Key))
        {
            string json = PlayerPrefs.GetString(Key);
            Loaded = JsonUtility.DeserializeObject<SaveData>(json);
        }
        else
        {
            Loaded = new SaveData();
        }

        AddBasicTower();

        foreach (var tower in _towerDB.Towers)
        {
            int id = tower.Data.ID;

            if(!Loaded.TowerLevelDict.ContainsKey(id))
            {
                Loaded.TowerLevelDict[id] = 1;
            }

            if(Loaded.GainedTowerID.Contains(id))
            {
                _towerDB.ActiveTowers.Add(_towerDB.GetTowerByID(id));
            }

            tower.Data.Level = Loaded.TowerLevelDict[id];
        }

        Save();
    }

    private void AddBasicTower()
    {
        foreach (var towerData in _towerDB.Towers)
        {
            int id = towerData.Data.ID;

            if (towerData.Data.Grade == 1 && !Loaded.GainedTowerID.Contains(id))
            {
                Loaded.GainedTowerID.Add(id);
            }

            if(!Loaded.TowerLevelDict.ContainsKey(id))
            {
                Loaded.TowerLevelDict[id] = 1;
            }

            towerData.Data.Level = Loaded.TowerLevelDict[id];
        }
    }

    public void AddGainedTowerID(int id)
    {
        if (!Loaded.GainedTowerID.Contains(id))
        {
            Loaded.GainedTowerID.Add(id);
        }
    }

    public void UpdateGem(int gems)
    {
        Loaded.Gem = gems;
    }

    public void UpdateReachedStage()
    {
        Loaded.ReachedStage += 1;
        Save();
    }

    public void Save()
    {
        string json = JsonUtility.SerializeObject(Loaded);
        PlayerPrefs.SetString(Key, json);
        PlayerPrefs.Save();
    }

    public void Remove()
    {
        PlayerPrefs.DeleteKey(Key);
        _towerDB.ActiveTowers.Clear();
    }

    public void Reset()
    {
        Remove();
        Load();
    }
}