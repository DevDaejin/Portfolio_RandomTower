using System;
using UnityEngine;

public class InGameUIHandler
{
    private InGameContext _context;
    
    private Action _onWave;
    private Action _onSpawnTower;

    private Action _onMerge;
    private Action _onSell;

    private Action _onRetry;
    private Action _onGoToLobby;

    public InGameUIHandler(InGameContext context, Action onWave, Action onSpawnTower, Action onMerge, Action onSell, Action onRetry, Action onGoToLobby)
    {
        _context = context;
        _onWave = onWave;
        _onSpawnTower = onSpawnTower;

        _onMerge = onMerge;
        _onSell = onSell;

        _onRetry = onRetry;
        _onGoToLobby = onGoToLobby;
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
            _onSell);

        _context.UI.WaveButton.onClick.RemoveAllListeners();
        _context.UI.WaveButton.onClick.AddListener(() =>_onWave?.Invoke());

        _context.UI.SpawnButton.onClick.RemoveAllListeners();
        _context.UI.SpawnButton.onClick.AddListener(() => _onSpawnTower?.Invoke());

        _context.UI.SetResultButtons(_onRetry, _onGoToLobby);
    }

    public int RefreshAlivedEnemyCount()
    {
        int count = _context.Enemy.GetAlivedEnemyCount;
        _context.UI.SetEnemyCount(count, _context.Wave.MaxEnemies);
        return count;
    }

    public void InteractableWaveButton(bool isAct)
    {
        _context.UI.InteractableWaveButton(isAct);
    }

    public void SelectTowerUI(GameObject gameObject)
    {
        if (gameObject == null) return;

        _context.UI.ActiveTowerOptionMenuUI(true);
        _context.UI.MoveTowerOptionMenuUI(gameObject.transform.position);
    }

    public void DeselectTowerUI()
    {
        _context.UI.ActiveTowerOptionMenuUI(false);
    }

    public void Reset()
    {

    }
}
