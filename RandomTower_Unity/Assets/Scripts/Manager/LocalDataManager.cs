using System;
using System.Collections.Generic;
using UnityEngine;


public class LocalDataManager
{
    [Serializable]
    public class SaveData
    {
        public List<string> GainedTowerNames = new();
        public int Gem = 0;
        public int ReachedStage = 1;
    }

    public SaveData Data { get; private set; }

    private const string BasicTower = "1_Grade_Archer";
    private const string Key = "LocalProgress";

    public SaveData Load()
    {
        if(PlayerPrefs.HasKey(Key))
        {
            string json = PlayerPrefs.GetString(Key);
            Data = JsonUtility.DeserializeObject<SaveData>(json);
        }
        else
        {
            Data = new();

            Data.Gem = 3;
            Data.ReachedStage = 1;
            Data.GainedTowerNames.Add(BasicTower);
        }

        return Data;
    }

    public void Save( )
    {
        string json = JsonUtility.SerializeObject(Data);
        PlayerPrefs.SetString(Key, json);
        PlayerPrefs.Save();
    }
}
 
