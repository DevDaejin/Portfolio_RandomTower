using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerGridController
{
    private const int MaxTowerCount = 3;
    private readonly List<TowerGrid> _grids = new();

    public TowerGridController(Transform[] grid, IDrag dragIndicator)
    {
        for (int index = 0; index < grid.Length; index++)
        {
            TowerGrid created = grid[index].AddComponent<TowerGrid>();
            created.Initialize(dragIndicator);
            _grids.Add(created);
        }
    }

    public TowerGrid GetTowerInstallableGrid(TowerData data)
    {
        TowerGrid availableGrid = null;

        if (!data.IsUnique)
        {
            availableGrid = GetSameTowerInstalledGrid(data);
        }

        if (availableGrid == null)
        {
            availableGrid = GetNullTowerGrid();
        }

        return availableGrid;
    }

    public TowerGrid GetGridDifferentID(TowerData data)
    {
        List<TowerGrid> candidates = new();

        foreach (TowerGrid grid in _grids)
        {
            var tower = grid.GetTower();
            if (tower == null) continue;

            if (tower.Data.Grade == data.Grade &&
                tower.Data.ID != data.ID &&
                grid.GetTowerCount() < MaxTowerCount)
            {
                candidates.Add(grid);
            }
        }

        if (candidates.Count == 0) return null;
        return candidates[Random.Range(0, candidates.Count)];
    }

    private TowerGrid GetSameTowerInstalledGrid(TowerData data)
    {
        TowerGrid[] installableGrids = GetSameTowerGrids(data);

        if (installableGrids == null || installableGrids.Length == 0) return null;

        int count = 0;
        for (int i = 0; i < installableGrids.Length; i++)
        {
            if (installableGrids[i].GetTowerCount() < MaxTowerCount)
                count++;
        }

        TowerGrid[] filtered = new TowerGrid[count];
        int index = 0;

        for (int i = 0; i < installableGrids.Length; i++)
        {
            if (installableGrids[i].GetTowerCount() < MaxTowerCount)
                filtered[index++] = installableGrids[i];
        }

        installableGrids = filtered;

        if (installableGrids.Length == 0) return null;

        int rand = Random.Range(0, installableGrids.Length);
        return installableGrids[rand];
    }

    private TowerGrid[] GetSameTowerGrids(TowerData data)
    {
        int count = 0;
        for (int i = 0; i < _grids.Count; i++)
        {
            var tower = _grids[i].GetTower();
            if (tower != null && tower.Data.ID == data.ID)
                count++;
        }

        TowerGrid[] sameTowerGrids = new TowerGrid[count];
        int index = 0;

        for (int i = 0; i < _grids.Count; i++)
        {
            var tower = _grids[i].GetTower();
            if (tower != null && tower.Data.ID == data.ID)
                sameTowerGrids[index++] = _grids[i];
        }
        return sameTowerGrids;
    }

    public TowerGrid GetNullTowerGrid()
    {
        int count = 0;
        for (int i = 0; i < _grids.Count; i++)
        {
            if (_grids[i].GetTowerCount() == 0)
                count++;
        }

        TowerGrid[] nullGrids = new TowerGrid[count];
        int index = 0;

        for (int i = 0; i < _grids.Count; i++)
        {
            if (_grids[i].GetTowerCount() == 0)
                nullGrids[index++] = _grids[i];
        }

        if (nullGrids.Length == 0) return null;

        int rand = Random.Range(0, nullGrids.Length);
        return nullGrids[rand];
    }

    public TowerGrid GetGridByPosition(Vector3 gridPosition)
    {
        foreach (var grid in _grids)
        {
            if (grid.transform.position == gridPosition)
            {
                return grid;
            }
        }

        return null;
    }

    public void RemoveTowerInGrid(BaseTower tower)
    {
        foreach(var grid in _grids)
        {
            foreach(var towerInGrid in grid.GetTowerList)
            {
                if(towerInGrid.GetInstanceID() == tower.GetInstanceID())
                {
                    grid.RemoveTower(tower);
                    return;
                }
            }
        }
    }

    public void RemoveAllTower()
    {
        foreach (TowerGrid grid in _grids)
        {
            grid.RemoveTowerAll();
        }
    }
}
