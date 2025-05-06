using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerDatabase", menuName = "Random TD/TowerDatabase")]
public class TowerDatabase : ScriptableObject
{
    public List<TowerData> _towers;

    public TowerData GetTowerByID(int id)
    {
        return _towers.FirstOrDefault(tower => tower.ID == id);
    }

    public List<TowerData> GetTowersByGrade(int grade)
    {
        return _towers.Where(tower => tower.Grade == grade).ToList();
    }
}
