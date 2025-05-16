using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GridController
{
    private const int MaxTowerCount = 3;
    private readonly List<Grid> _grids = new();

    public GridController(Transform[] grid)
    {
        for (int index = 0; index < grid.Length; index++)
        {
            Grid created = grid[index].AddComponent<Grid>();
            created.Initialize();
            _grids.Add(created);
        }
    }

    public Grid GetTowerInstallableGrid(TowerData data)
    {
        Grid availableGrid = null;

        if (!data.IsSpecial)
        {
            availableGrid = GetSameTowerInstalledGrid(data);
        }

        if (availableGrid == null)
        {
            availableGrid = GetNullTowerGrid();
        }

        return availableGrid;
    }

    public Grid GetGridDifferentID(TowerData data)
    {
        List<Grid> candidates = new();

        foreach (Grid grid in _grids)
        {
            ITower tower = grid.GetTower();
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

    private Grid GetSameTowerInstalledGrid(TowerData data)
    {
        Grid[] installableGrids = GetSameTowerGrids(data);

        if (installableGrids == null || installableGrids.Length == 0) return null;

        int count = 0;
        for (int i = 0; i < installableGrids.Length; i++)
        {
            if (installableGrids[i].GetTowerCount() < MaxTowerCount)
                count++;
        }

        Grid[] filtered = new Grid[count];
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

    private Grid[] GetSameTowerGrids(TowerData data)
    {
        int count = 0;
        for (int i = 0; i < _grids.Count; i++)
        {
            var tower = _grids[i].GetTower();
            if (tower != null && tower.Data.ID == data.ID)
                count++;
        }

        Grid[] sameTowerGrids = new Grid[count];
        int index = 0;

        for (int i = 0; i < _grids.Count; i++)
        {
            var tower = _grids[i].GetTower();
            if (tower != null && tower.Data.ID == data.ID)
                sameTowerGrids[index++] = _grids[i];
        }
        return sameTowerGrids;
    }

    private Grid GetNullTowerGrid()
    {
        int count = 0;
        for (int i = 0; i < _grids.Count; i++)
        {
            if (_grids[i].GetTowerCount() == 0)
                count++;
        }

        Grid[] nullGrids = new Grid[count];
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
}
