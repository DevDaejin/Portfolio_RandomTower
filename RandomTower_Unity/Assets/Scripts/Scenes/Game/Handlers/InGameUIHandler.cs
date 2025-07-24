public class InGameUIHandler
{
    private InGameContext _context;
    private InGameUISetting _setting;

    public InGameUIHandler(InGameContext context, InGameUISetting setting)
    {
        _context = context;
        _setting = setting;
    }

    public void Initialize()
    {
        _setting.MaxWave = _context.Wave.MaxWave;
        _setting.MaxEnemy = _context.Wave.MaxEnemies;
        _setting.MaxTower = _context.Tower.MaxTower;
        _setting.Time = _context.Wave.WaveDuration;
        _setting.Gold = _context.Resource.Get(ResourceManager.ResourceType.Gold);
        _setting.IsHost = _context.Network.IsHost;

        _context.UI.Initialize(_setting);

        _context.UI.WaveButton.onClick.AddListener(() => _setting.OnWave?.Invoke());
        _context.UI.SpawnButton.onClick.AddListener(() => _setting.OnSpawnTower?.Invoke());
        _context.UI.SetResultButtons(_setting.OnRetry, _setting.OnGoToLobby);

        _context.UI.UpgradeButton.onClick.AddListener(() => _setting.OnUpgrade.Invoke());
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

    public void SetInteractableMergeButton(bool isAct) => _context.UI.SetInteractableMergeButton(isAct);

    public void SetInteractableWaveButton(bool isAct) => _context.UI.SetInteractableWaveButton(isAct);

    public void SetInteractableUpgradeButton(bool isAct) => _context.UI.SetInteractableUpgradeButton(isAct);

    public void SelectTowerUI(BaseTower tower, bool isMergeable)
    {
        if (tower == null) return;

        _context.UI.ActiveTowerOptionMenuUI(true);
        _context.UI.SetInteractableMergeButton(isMergeable);
    }

    public void SetGold(int amount) => _context.UI.SetGoldCount(amount);

    public void DeselectTowerUI() => _context.UI.ActiveTowerOptionMenuUI(false);

    public void Reset() => Initialize();
}
