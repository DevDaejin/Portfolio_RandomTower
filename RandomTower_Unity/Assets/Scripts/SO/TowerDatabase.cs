using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerDatabase", menuName = "Random TD/TowerDatabase")]
public class TowerDatabase : ScriptableObject
{
    public List<TowerDataConfig> _towers;

    public TowerData GetTowerByID(int id)
    {
        return _towers.Select(tower => tower.Data).FirstOrDefault(data => data.ID == id);
    }

    public TowerData[] GetTowersByGrade(int grade)
    {
        return _towers.Select(tower => tower.Data).Where(data => data.Grade == grade).ToArray();
    }
}
