using System;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class InGameUIHandler
{
    private InGameContext _context;
    private Action _onWave;
    private Action _onSpawnTower;

    public InGameUIHandler(InGameContext context, Action onWave, Action onSpawnTower)
    {
        _context = context;
        _onWave = onWave;
        _onSpawnTower = onSpawnTower;
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
            null,
            null);

        _context.UI.WaveButton.onClick.AddListener(() =>_onWave?.Invoke());
        _context.UI.SpawnButton.onClick.AddListener(() => _onSpawnTower.Invoke());
        _context.UI.SetResultButtons(Retry, GoToLobby);
    }
}
