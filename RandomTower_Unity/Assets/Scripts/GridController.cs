using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridController
{
    private List<Grid> _grids = new();

    public GridController(Transform[] grid)
    {
        for (int index = 0; index < grid.Length; index++)
        {
            _grids.Add(new Grid(grid[index]));
        }
    }

    public Grid GetTowerInstallableGrid(BaseTower tower)
    {
        Grid availabaleGrid = null;
        
        if (!tower.IsSpecial)
        {
            availabaleGrid = GetSameTowerInstalledGrid(tower);
        }

        if(availabaleGrid == null)
        {
            availabaleGrid = GetNullTowerGrid();
        }

        return availabaleGrid;
    }

    private Grid GetSameTowerInstalledGrid(BaseTower tower)
    {
        Grid[] installableGrids = GetSameTowerGrids(tower);

        if(installableGrids == null || installableGrids.Length == 0) return null;

        installableGrids = installableGrids.Where(grid => grid.GetTowerCount() < 3).ToArray();

        if (installableGrids.Length == 0) return null;

        int rand = Random.Range(0, installableGrids.Length);
        return installableGrids[rand];
    }

    private Grid[] GetSameTowerGrids(BaseTower tower)
    {
        Grid[] sameTowerGrids = _grids.Where(grid => grid.GetTower() != null && grid.GetTower().GetID == tower.GetID).ToArray();
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
