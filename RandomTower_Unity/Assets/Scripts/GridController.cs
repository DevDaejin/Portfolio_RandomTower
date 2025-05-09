using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridController
{
    private const int MaxTowerCount = 3;
    private List<Grid> _grids = new();
    

    public GridController(Transform[] grid)
    {
        for (int index = 0; index < grid.Length; index++)
        {
            _grids.Add(new Grid(grid[index]));
        }
    }

    public Grid GetTowerInstallableGrid(TowerDataConfig config)
    {
        Grid availabaleGrid = null;

        if (!config.Data.IsSpecial)
        {
            availabaleGrid = GetSameTowerInstalledGrid(config);
        }

        if (availabaleGrid == null)
        {
            availabaleGrid = GetNullTowerGrid();
        }

        return availabaleGrid;
    }

    private Grid GetSameTowerInstalledGrid(TowerDataConfig configata)
    {
        Grid[] installableGrids = GetSameTowerGrids(configata);

        if(installableGrids == null || installableGrids.Length == 0) return null;

        installableGrids = installableGrids.Where(grid => grid.GetTowerCount() < MaxTowerCount).ToArray();

        if (installableGrids.Length == 0) return null;

        int rand = Random.Range(0, installableGrids.Length);
        return installableGrids[rand];
    }

    private Grid[] GetSameTowerGrids(TowerDataConfig config)
    {
        Grid[] sameTowerGrids = _grids.Where(grid => grid.GetTower() != null && grid.GetTower().Data.ID == config.Data.ID).ToArray();
        return sameTowerGrids;
    }

    private Grid GetNullTowerGrid()
    {
        Grid[] nullGrids = _grids.Where(grid=> grid.GetTowerCount() == 0).ToArray();
        
        if (nullGrids.Length == 0) return null;

        int rand = Random.Range(0, nullGrids.Length);
        return nullGrids[rand];
    }
}
