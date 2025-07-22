using System;
using UnityEngine;

public class WaveSetting
{
    public StageConfig StageConfig;
    public int MaxWave;
    public int MaxEnemies;
    public float WaveDuration;
    
    public Func<bool> GetSpawningState;
    public Func<int> GetAliveEnemyCount;

    public Action<float> OnTimeChanged;
    public Action<int, int> OnWaveChanged;
    public Action<int, int> OnEnemyCountChanged;

    public Action OnWaveStarted;
    public Action OnWaveEnded;

    public Action<bool> OnStageResult;
}
