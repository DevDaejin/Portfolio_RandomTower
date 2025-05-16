using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerDatabase", menuName = "Random TD/TowerDatabase")]
public class TowerDatabase : ScriptableObject
{
    public List<TowerDataConfig> _towers;

    public TowerData GetTowerByID(int id)
    {
        for (int i = 0; i < _towers.Count; i++)
        {
            TowerData data = _towers[i].Data;
            if (data.ID == id)
            {
                return data;
            }
        }
        return null;
    }

    public TowerData[] GetTowersByGrade(int grade)
    {
        int count = 0;
        for (int i = 0; i < _towers.Count; i++)
        {
            if (_towers[i].Data.Grade == grade)
            {
                count++;
            }
        }

        TowerData[] result = new TowerData[count];
        int index = 0;

        for (int i = 0; i < _towers.Count; i++)
        {
            if (_towers[i].Data.Grade == grade)
            {
                result[index++] = _towers[i].Data;
            }
        }

        return result;
    }
}
