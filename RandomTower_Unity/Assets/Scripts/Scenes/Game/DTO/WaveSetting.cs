using System;

public class WaveSetting
{
    public StageConfig StageConfig = null;
    public int MaxWave = 0;
    public int MaxEnemies = 0;
    public float WaveDuration = 0;

    public Func<bool> GetSpawningState = null;
    public Func<int> GetAliveEnemyCount = null;

    public Action<float> OnTimeChanged = null;
    public Action<int, int> OnWaveChanged = null;
    public Action<int, int> OnEnemyCountChanged = null;

    public Action OnWaveStarted = null;
    public Action OnWaveEnded = null;

    public Action<bool> OnStageResult = null;
}
