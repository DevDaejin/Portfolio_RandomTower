using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class LocalDataManager
{
    public LocalDataManager(List<string> names)
    {
        if (!PlayerPrefs.HasKey(Key))
        { 
            Loaded = new();
            Loaded.GainedTowerNames.AddRange(names);
            //TODO : 추 후 업데이트
            //Save();
        }
    }

    [Serializable]
    public class SaveData
    {
        public List<string> GainedTowerNames = new();
        public int Gem = BasicGem;
        public int ReachedStage = BasicReachedStage;
    }

    public SaveData Loaded { get; private set; }

    private const int BasicGem = 3;
    private const int BasicReachedStage = 1; 
    private const string Key = "LocalProgress";

    public SaveData Load(string[] basicTowers)
    {
        string json = PlayerPrefs.GetString(Key);
        Loaded = JsonUtility.DeserializeObject<SaveData>(json);
        return Loaded;
    }

    public void Save( )
    {
        string json = JsonUtility.SerializeObject(Loaded);
        PlayerPrefs.SetString(Key, json);
        PlayerPrefs.Save();
    }
}
 
