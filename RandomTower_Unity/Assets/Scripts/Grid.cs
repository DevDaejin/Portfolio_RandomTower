using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private readonly Transform _transform;
    private int _maxCount;

    private List<BaseTower> towers;

    private const int MaxSpecial = 1;
    private const int MaxNormal = 3;

    public Grid(Transform t)
    {
        _transform = t;
        towers = new();
    }

    public bool TryAddTower(BaseTower tower)
    {
        //TODO : 스페셜 타워, 현재 타워 갯수 체크 로직 필요
        //_maxCount = tower.IsSpecial ? MaxSpecial : MaxNormal;

        if (towers.Count < _maxCount)
        {
            towers.Add(tower);
            tower.transform.position = _transform.position;
            return true;
        }

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
