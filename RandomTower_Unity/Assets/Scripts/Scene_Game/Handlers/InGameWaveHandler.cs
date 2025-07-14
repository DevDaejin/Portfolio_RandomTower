using Game;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InGameWaveHandler
{
    private InGameContext _context;
    private StageConfig _stageConfig;
    private Action _onWave;
    public InGameWaveHandler(InGameContext context, StageConfig stageConfig, Action onWave)
    {
        _context = context;
        _stageConfig = stageConfig;
        _onWave = onWave;
    }

    public void Initialize()
    {
        _context.Wave.OnTimeChanged += _context.UI.SetTimer;
        _context.Wave.OnWaveChanged += _context.UI.SetWave;
        _context.Wave.OnEnemyCountChanged += _context.UI.SetEnemyCount;
        _context.Wave.OnStageResult += Result;
        _context.Wave.OnWaveEnded += _onWave;
        _context.Wave.OnWaveStarted += OnWaveStarted;
        _context.Wave.Initialize();
    }



    private void OnWaveStarted()
    {
        _context.Enemy.SpawnWave(_stageConfig, _context.Wave.CurrentWaveIndex);
    }

    private void Result(bool isSuccess)
    {
        if (isSuccess)
        {
            _context.UI.SetResult(true);
        }
        else
        {
            _context.UI.SetResult(false);
        }
    }
}
