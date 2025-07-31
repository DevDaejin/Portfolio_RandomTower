using System;
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
        public GameSaveData Game = new();
        public OptionSaveData Option = new();
    }

    public SaveData SavedData { get; private set; } = new SaveData();


    private const string GameKey = "game";
    private const string OptionKey = "option";

    public void Load(string key = null)
    {
        if (string.IsNullOrEmpty(key))
        {
            SavedData.Game = LoadGame();
            Save(GameKey);

            SavedData.Option = LoadOption();
            Save(OptionKey);
        }
        else
        {
            if (key == GameKey)
            {
                SavedData.Game = LoadGame();
                Save(GameKey);
            }

            if (key == OptionKey)
            {
                SavedData.Option = LoadOption();
                Save(OptionKey);
            }
        }
    }

    private GameSaveData LoadGame()
    {
        if (PlayerPrefs.HasKey(GameKey))
        {
            string json = PlayerPrefs.GetString(GameKey);
            return JsonUtility.DeserializeObject<GameSaveData>(json);
        }

        var gameData = new GameSaveData();

        foreach (var tower in _towerDB.Towers)
        {
            int id = tower.Data.ID;

            if (tower.Data.Grade == 1 && !gameData.GainedTowerID.Contains(id))
            {
                gameData.GainedTowerID.Add(id);
            }

            if (!gameData.TowerLevelDict.ContainsKey(id))
            {
                gameData.TowerLevelDict[id] = 1;
            }

            if (gameData.GainedTowerID.Contains(id))
            {
                _towerDB.ActiveTowers.Add(_towerDB.GetTowerByID(id));
            }

            tower.Data.Level = gameData.TowerLevelDict[id];
        }

        return gameData;
    }

    private OptionSaveData LoadOption()
    {
        if (PlayerPrefs.HasKey(OptionKey))
        {
            string json = PlayerPrefs.GetString(OptionKey);
            return JsonUtility.DeserializeObject<OptionSaveData>(json);
        }

        var optionData = new OptionSaveData();
        return optionData;
    }

    public void AddGainedTowerID(int id)
    {
        if (!SavedData.Game.GainedTowerID.Contains(id))
        {
            SavedData.Game.GainedTowerID.Add(id);
        }
    }

    public void UpdateGem(int gems)
    {
        SavedData.Game.Gem = gems;
    }

    public void UpdateReachedStage()
    {
        SavedData.Game.ReachedStage += 1;
        Save(GameKey);
    }

    public void SaveGame() => Save(GameKey);
    public void SaveOption() => Save(OptionKey);

    private void Save(string key)
    {
        string json = string.Empty;
        if (key == GameKey)
        {
            json = JsonUtility.SerializeObject(SavedData.Game);
        }
        if (key == OptionKey)
        {
            json = JsonUtility.SerializeObject(SavedData.Option);
        }

        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    public void Remove(string key)
    {
        PlayerPrefs.DeleteKey(key);

        if (key == GameKey)
        {
            _towerDB.ActiveTowers.Clear();
            SavedData.Game = new();
        }
        if (key == OptionKey)
        {
            SavedData.Option = new();
        }
    }

    public void Reset()
    {
        Remove(GameKey);
        Load();
    }
}