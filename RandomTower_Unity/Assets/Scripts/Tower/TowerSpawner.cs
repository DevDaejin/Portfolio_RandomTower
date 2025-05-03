using System.Linq;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private Transform installationGrid;
    private GridController gridController;

    [SerializeField] private GameObject testTower;

    private void Awake()
    {
        Transform[] tree = installationGrid.GetComponentsInChildren<Transform>();
        tree = tree.Where(branch => branch != installationGrid).ToArray();

        gridController = new GridController(tree);
    }

    public void SpawnTower()
    {
        var tower = Instantiate(testTower).GetComponent<BaseTower>();
        tower.GetID = 1;
        TryInstallTower(tower);
    }

    private bool TryInstallTower(BaseTower tower)
    {
        Grid grid = gridController.GetTowerInstallableGrid(tower);

        if (grid == null) return false;

        return grid.TryAddTower(tower);
    }
}
