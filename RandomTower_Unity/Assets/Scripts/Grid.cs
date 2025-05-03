using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private readonly Transform _transform;
    private int _maxCount;

    private List<BaseTower> towers;
    


    public Grid(Transform t)
    {
        _transform = t;
        towers = new();
    }

    public bool TryAddTower(BaseTower tower)
    {
        if (towers.Count < _maxCount)
        {
            towers.Add(tower);
            return true;
        }

        tower.transform.position = _transform.position;

        return false;
    }
    public int GetTowerCount()
    {
        return towers.Count;
    }

    public BaseTower GetTower()
    {
        if (towers.Count <= 0) return null;

        return towers[0];
    }
}
