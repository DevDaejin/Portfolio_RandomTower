using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerDatabase", menuName = "Random TD/TowerDatabase")]
public class TowerDatabase : ScriptableObject
{
    public List<TowerDataConfig> Towers;
    public List<TowerDataConfig> ActiveTowers = new();
    public List<TowerCombinationData> Combinations = new();
    private Dictionary<int, TowerDataConfig> _idDict = null;
    private Dictionary<string, TowerDataConfig> _nameDict = null;


    private void Initialize()
    {
        if (_idDict != null && _nameDict != null) return;

        _idDict = new();
        _nameDict = new();

        foreach (var tower in Towers)
        {
            _idDict[tower.Data.ID] = tower;
            _nameDict[tower.Data.TowerName] = tower;
        }
    }

    public TowerDataConfig GetTowerByID(int id)
    {
        Initialize();
        return _idDict.TryGetValue(id, out var data) ? data : null;
    }

    public TowerDataConfig[] GetTowersByGrade(int grade)
    {
        Initialize();
        List<TowerDataConfig> result = new();

        foreach (var tower in ActiveTowers)
        {
            if (tower.Data.Grade == grade)
            {
                result.Add(tower);
            }
        }

        return result.ToArray();
    }

    public List<TowerCombinationData> GetAvailableCombinations(List<int> installedIds)
    {
        List<TowerCombinationData> results = new();

        for (int i = 0; i < Combinations.Count; i++)
        {
            TowerCombinationData combinationData = Combinations[i];

            List<int> requiredIds = new();
            for (int j = 0; j < combinationData.RequiredTowers.Count; j++)
            {
                requiredIds.Add(combinationData.RequiredTowers[j].Data.ID);
            }

            if (CheckContainsRequiredIds(installedIds, requiredIds))
            {
                results.Add(combinationData);
            }
        }

        return results;
    }

    public bool TryGetCombination(List<int> ids, out int resultId)
    {
        for (int i = 0; i < Combinations.Count; i++)
        {
            TowerCombinationData combination = Combinations[i];

            List<int> requiredIds = new();
            for (int j = 0; j < combination.RequiredTowers.Count; j++)
            {
                requiredIds.Add(combination.RequiredTowers[j].Data.ID);
            }

            if (CheckContainsRequiredIds(ids, requiredIds))
            {
                resultId = combination.ResultTower.Data.ID;
                return true;
            }
        }

        resultId = -1;
        return false;
    }

    private bool CheckContainsRequiredIds(List<int> ids, List<int> requiredIds)
    {
        Dictionary<int, int> countDict = new();

        foreach (var id in ids)
        {
            if (!countDict.ContainsKey(id))
            {
                countDict[id] = 0;
            }

            countDict[id]++;
        }

        foreach (var id in requiredIds)
        {
            if (!countDict.ContainsKey(id) || countDict[id] == 0)
            {
                return false;
            }

            countDict[id]--;
        }


        return true;
    }


    public TowerDataConfig GetTowerByName(string name)
    {
        Initialize();
        return _nameDict.TryGetValue(name, out var data) ? data : null;
    }
}
