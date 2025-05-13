using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GridController
{
    private const int MaxTowerCount = 3;
    private List<Grid> _grids = new();


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
        Grid availabaleGrid = null;

        if (!data.IsSpecial)
        {
            availabaleGrid = GetSameTowerInstalledGrid(data);
        }

        if (availabaleGrid == null)
        {
            availabaleGrid = GetNullTowerGrid();
        }

        return availabaleGrid;
    }

    public Grid GetGridDifferentID(TowerData data)
    {
        Grid[] candidates = _grids.Where(grid =>
            grid.GetTower() != null &&
            grid.GetTower().Data.Grade == data.Grade &&
            grid.GetTower().Data.ID != data.ID &&
            grid.GetTowerCount() < MaxTowerCount).ToArray();

        if (candidates.Length == 0) return null;
        return candidates[Random.Range(0, candidates.Length)];
    }

    private Grid GetSameTowerInstalledGrid(TowerData data)
    {
        Grid[] installableGrids = GetSameTowerGrids(data);

        if (installableGrids == null || installableGrids.Length == 0) return null;

        installableGrids = installableGrids.Where(grid => grid.GetTowerCount() < MaxTowerCount).ToArray();

        if (installableGrids.Length == 0) return null;

        int rand = Random.Range(0, installableGrids.Length);
        return installableGrids[rand];
    }

    private Grid[] GetSameTowerGrids(TowerData data)
    {
        Grid[] sameTowerGrids = _grids.Where(grid => grid.GetTower() != null && grid.GetTower().Data.ID == data.ID).ToArray();
        return sameTowerGrids;
    }

    private Grid GetNullTowerGrid()
    {
        Grid[] nullGrids = _grids.Where(grid => grid.GetTowerCount() == 0).ToArray();

        if (nullGrids.Length == 0) return null;

        int rand = Random.Range(0, nullGrids.Length);
        return nullGrids[rand];
    }
}
