using System;
using UnityEngine;

public class InGameUIHandler
{
    private InGameContext _context;
    
    private Action _onWave;
    private Action _onSpawnTower;
    private Action _onUpgrade;

    private Action _onMerge;
    private Action _onSell;

    private Action _onRetry;
    private Action _onGoToLobby;

    private Func<int> _onSpawnPrice;
    private Func<int> _onUpgradePrice;
    private Func<int[]> _onUpgradeInfo;

    public InGameUIHandler(InGameContext context, 
        Action onWave, Action onSpawnTower, Action onUpgrade,
        Action onMerge, Action onSell, 
        Action onRetry, Action onGoToLobby, 
        Func<int> onSpawnPrice, Func<int>onUpgradePrice, Func<int[]> onUpgradeInfo)
    {
        _context = context;

        _onWave = onWave;
        _onSpawnTower = onSpawnTower;
        _onUpgrade = onUpgrade;

        _onMerge = onMerge;
        _onSell = onSell;

        _onRetry = onRetry;
        _onGoToLobby = onGoToLobby;

        _onSpawnPrice = onSpawnPrice;
        _onUpgradePrice = onUpgradePrice;
        _onUpgradeInfo = onUpgradeInfo;
    }

    public void Initialize()
    {
        _context.UI.Initialize(
            _context.Wave.MaxWave, 
            _context.Wave.MaxEnemies, 
            _context.Tower.MaxTower, 
            _context.Wave.WaveDuration, 
            _context.Resource.Get(ResourceManager.ResourceType.Gold), 
            _context.Network.IsHost,
            _onMerge,
            _onSell,
            _onSpawnPrice,
            _onUpgradePrice,
            _onUpgradeInfo);

        _context.UI.WaveButton.onClick.AddListener(() =>_onWave?.Invoke());
        _context.UI.SpawnButton.onClick.AddListener(() => _onSpawnTower?.Invoke());
        _context.UI.SetResultButtons(_onRetry, _onGoToLobby);

        _context.UI.UpgradeButton.onClick.AddListener(() => _onUpgrade.Invoke());
    }

    public int RefreshAlivedEnemyCount()
    {
        int count = _context.Enemy.GetAlivedEnemyCount;
        _context.UI.SetEnemyCount(count, _context.Wave.MaxEnemies);
        return count;
    }

    public void RefreshInstalledTowerCount()
    {
        int count = _context.Tower.InstalledCount;
        _context.UI.SetTowerCount(count, _context.Tower.MaxTower);
    }

    public void SetInteractableMergeButton(bool isAct) => _context.UI.SetInterableMergeButton(isAct);

    public void SetInteractableWaveButton(bool isAct) => _context.UI.SetInteractableWaveButton(isAct);

    public void SelectTowerUI(BaseTower tower, bool isMergeable)
    {
        if (tower == null) return;

        _context.UI.ActiveTowerOptionMenuUI(true);
        _context.UI.SetInterableMergeButton(isMergeable);
        _context.UI.MoveTowerOptionMenuUI(tower.transform.position);
    }

    public void SetGold(int amount) => _context.UI.SetGoldCount(amount);

    public void DeselectTowerUI() => _context.UI.ActiveTowerOptionMenuUI(false);

    public void Reset() => Initialize();
}
