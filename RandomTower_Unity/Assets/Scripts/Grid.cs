using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private readonly Transform _transform;
    private int _maxCount = 3;
    private List<ITower> _towers;

    private const float LeftSideX = -0.2f;
    private const float RightSideX = 0.2f;
    private const float intervalZ = 0.2f;

    private static readonly Vector3[] Positions3 = new[]
    {
        new Vector3(LeftSideX, 0, intervalZ),
        new Vector3(RightSideX, 0, 0),
        new Vector3(LeftSideX, 0, -intervalZ),
    };

    private static readonly Vector3[] Positions2 = new[]
    {
        new Vector3(LeftSideX, 0, intervalZ * 0.5f),
        new Vector3(RightSideX, 0, -intervalZ * 0.5f),
    };

    public Grid(Transform transform)
    {
        _transform = transform;
        _towers = new();
    }

    public bool TryAddTower(ITower tower)
    {
        if ((_towers.Count == 0 ||_towers[0].Data.IsSpecial) 
            && _towers.Count < _maxCount)
        {
            _towers.Add(tower);
            UpdateTowerPosition();
            return true;
        }

        return false;
    }

    public int GetTowerCount()
    {
        return _towers.Count;
    }

    public ITower GetTower()
    {
        return (_towers.Count > 0) ? _towers[0] : null;
    }

    private void UpdateTowerPosition()
    {
        if (_towers.Count == 1)
        {
            _towers[0].Transform.position = _transform.position;
        }
        else
        {
            Vector3[] offsets = _towers.Count switch
            {
                2 => Positions2,
                3 => Positions3,
                _ => null
            };

            if (offsets == null) return;

            for (int index = 0; index < offsets.Length; index++)
            {
                _towers[index].Transform.position = _transform.position + offsets[index];
            }
        }
    }
}
