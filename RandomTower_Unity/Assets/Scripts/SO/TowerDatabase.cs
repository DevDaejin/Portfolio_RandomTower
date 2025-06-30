using System.Collections.Generic;
using System.Data;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerDatabase", menuName = "Random TD/TowerDatabase")]
public class TowerDatabase : ScriptableObject
{
    public List<TowerDataConfig> Towers;

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

        foreach (var tower in Towers)
        {
            if (tower.Data.Grade == grade)
            {
                result.Add(tower);
            }
        }

        return result.ToArray();
    }

    public TowerDataConfig GetTowersByName(string name)
    {
        Initialize();
        return _nameDict.TryGetValue(name, out var data) ? data : null;
    }
}
