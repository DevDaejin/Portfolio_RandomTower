using System;
using System.Collections.Generic;
using UnityEngine;


public class LocalDataManager
{
    private TowerDatabase _towerDB;

    public LocalDataManager(TowerDatabase towerDB)
    {
        //TODO : 임시 코드
        Remove();

        _towerDB = towerDB;

        if (!PlayerPrefs.HasKey(Key))
        { 
            Loaded = new();

            List<int> basicTowers = new();
            foreach(var towerData in _towerDB.Towers)
            {
                if(towerData.Data.Grade == 1)
                {
                    basicTowers.Add(towerData.Data.ID);
                }

                Loaded.TowerLevelDict[towerData.Data.ID] = 1;
                towerData.Data.Level = 1;
            }

            Loaded.GainedTowerID.AddRange(basicTowers);
            //TODO : 추 후 업데이트
            //Save();
        }
        else
        {
            Load();
        }
    }

    [Serializable]
    public class SaveData
    {
        public List<int> GainedTowerID = new();
        public Dictionary<int, int> TowerLevelDict = new();
        public int Gem = BasicGem;
        public int ReachedStage = BasicReachedStage;
    }

    public SaveData Loaded { get; private set; }

    private const int BasicGem = 3;
    private const int BasicReachedStage = 1; 
    private const string Key = "LocalProgress";

    public SaveData Load()
    {
        string json = PlayerPrefs.GetString(Key);
        Loaded = JsonUtility.DeserializeObject<SaveData>(json);

        foreach(var tower in _towerDB.Towers)
        {
            tower.Data.Level = Loaded.TowerLevelDict[tower.Data.ID];
        }

        return Loaded;
    }

    public void AddGainedTowerID(int id)
    {
        Loaded.GainedTowerID.Add(id);
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
    }

    public void Reset()
    {
        PlayerPrefs.DeleteAll();
    }
}
 
