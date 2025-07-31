using System.Collections.Generic;
using UnityEngine;

public class TowerGrid : MonoBehaviour, IDrag, ISelect
{
    private Transform _transform;
    private List<BaseTower> _towers;
    private IDrag _indicator;

    private const int MaxCount = 3;
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

    public void Initialize(IDrag indicator)
    {
        _transform = GetComponent<Transform>();
        _towers = new();
        _indicator = indicator;
    }

    public bool TryAddTower(BaseTower tower)
    {
        if (_towers.Count >= MaxCount || (_towers.Count > 0 && _towers[0].Data.IsUnique))
        {
            return false;
        }

        _towers.Add(tower);
        UpdateTowerPosition();
        return true;
    }

    public int GetTowerCount() => _towers.Count;

    public BaseTower GetTower() => (_towers.Count > 0) ? _towers[_towers.Count - 1] : null;

    public List<BaseTower> GetTowerList => _towers;

    public bool IsMergeable => _towers.Count == MaxCount && GetTower().Data.IsMergeableGrade;

    public void RemoveTower()
    {
        if (_towers.Count <= 0) return;
        _towers.Remove(GetTower());

        UpdateTowerPosition();
    }

    public void RemoveTower(BaseTower tower)
    {
        _towers.Remove(tower);
        UpdateTowerPosition();
    }

    public void RemoveTowerAll()
    {
        int count = _towers.Count;

        for (int i = 0; i < count; i++)
        {
            RemoveTower();
        }

        _towers.Clear();
    }

    private void UpdateTowerPosition()
    {
        int count = _towers.Count;
        if (count > 0)
        {
            var positions = GetTowerPostions(_towers.Count);
            for (int index = 0; index < _towers.Count; index++)
            {
                _towers[index].transform.position = positions[index];
            }
        }
    }

    public Vector3[] GetTowerPostions(int towerCount)
    {
        List<Vector3> positions = new();

        if (towerCount == 1)
        {
            positions.Add(_transform.position);
        }
        else
        {
            Vector3[] offsets = towerCount switch
            {
                1 => new Vector3[] { Vector3.zero },
                2 => Positions2,
                3 => Positions3,
                _ => null
            };

            for (int index = 0; index < offsets.Length; index++)
            {
                positions.Add(_transform.position + offsets[index]);
            }
        }

        return positions.ToArray();
    }

    public void OnSelect()
    {
        if (_towers.Count == 0) return;

        var lastTower = GetTower();
        foreach (var tower in _towers)
        {
            tower.ShowRange(lastTower == tower);
        }
    }

    public void OnDeselect()
    {
        if (_towers.Count == 0) return;

        foreach (var tower in _towers)
        {
            tower.ShowRange(false);
        }
    }

    public void OnBeginDrag(Vector3 startPosition)
    {
        if (GetTowerCount() == 0) return;
        _indicator.OnBeginDrag(startPosition);
    }

    public void OnDrag(Vector3 currentPosition)
    {
        if (GetTowerCount() == 0) return;
        _indicator.OnDrag(currentPosition);
    }

    public void OnEndDrag(Vector3 endPosition)
    {
        if (GetTowerCount() == 0) return;
        _indicator.OnEndDrag(endPosition);
    }
}
