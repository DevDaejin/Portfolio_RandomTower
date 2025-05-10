using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerDatabase", menuName = "Random TD/TowerDatabase")]
public class TowerDatabase : ScriptableObject
{
    public List<TowerDataConfig> _towers;

    public TowerDataConfig GetTowerByID(int id)
    {
        return _towers.FirstOrDefault(tower => tower.Data.ID == id);
    }

    public List<TowerDataConfig> GetTowersByGrade(int grade)
    {
        return _towers.Where(tower => tower.Data.Grade == grade).ToList();
    }
}
