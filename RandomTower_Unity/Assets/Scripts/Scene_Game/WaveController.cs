using System;
using UnityEngine;

public class WaveController
{
    public enum WaveState { Idle, InProgress, Waiting, Failed, Cleared }
    public WaveState CurrentState { get; private set; } = WaveState.Idle;
    public int CurrentWaveIndex { get; private set; }
    public bool IsFinalWave => (CurrentWaveIndex + 1) == _maxWave;

    private Timer _timer;

    public float WaveDuration => _waveDuration;
    private float _waveDuration;

    public int MaxWave => _maxWave;
    private int _maxWave;

    public int MaxEnemies => _maxEnemies;
    private int _maxEnemies;
    private Func<bool> _getSpawningState;
    private Func<int> _getAliveEnemyCount;

    public event Action<float> OnTimeChanged;
    public event Action<int, int> OnWaveChanged;
    public event Action<int, int> OnEnemyCountChanged;

    public event Action OnWaveStarted;
    public event Action OnWaveEnded;
    public event Action<bool> OnStageResult;

    public WaveController(int maxWave, int maxEnemies, float waveDuration, Func<bool> getSpawningState, Func<int> getAliveEnemyCount)
    {
        _maxWave = maxWave;
        _maxEnemies = maxEnemies;
        _getSpawningState = getSpawningState;
        _getAliveEnemyCount = getAliveEnemyCount;

        _waveDuration = waveDuration;
        _timer = new Timer(WaveDuration);
        _timer.OnTick += time => OnTimeChanged?.Invoke(time);
        _timer.OnTimeUp += OnTimeUp;
    }

    public void Initialize()
    {
        CurrentWaveIndex = 0;
        CurrentState = WaveState.Idle;
        _timer.Stop();

        OnWaveChanged.Invoke(CurrentWaveIndex, _maxWave);
        OnTimeChanged.Invoke(_timer.TimeLeft);
    }

    //TODO: 추후 삭제 테스트용
#if UNITY_EDITOR
    public void TestCode()
    {
        _timer.TimeLeft = 1;
    }
#endif

    public void StartWave()
    {
        if (CurrentState != WaveState.Idle) return;

        CurrentState = WaveState.InProgress;
        _timer.Start();

        OnWaveStarted?.Invoke();
        OnWaveChanged?.Invoke(CurrentWaveIndex + 1, _maxWave);
    }

    public void ForceTimeUp()
    {
        if (CurrentState != WaveState.InProgress) return;

        _timer.Stop();
        OnTimeUp();
    }

    public void EndWave()
    {
        CurrentWaveIndex++;

        if (CurrentWaveIndex >= _maxWave)
        {
            ClearStage();
            return;
        }

        CurrentState = WaveState.Idle;
        OnWaveEnded?.Invoke();
    }

    public void Update()
    {
        if (CurrentState != WaveState.InProgress) return;

        _timer.Tick();

        int alive = _getAliveEnemyCount.Invoke();
        OnEnemyCountChanged?.Invoke(alive, _maxEnemies);

        if (alive > _maxEnemies)
        {
            FailStage();
            return;
        }

        if (IsFinalWave && alive == 0 && !_getSpawningState.Invoke())
        {
            ClearStage();
        }
    }

    private void OnTimeUp()
    {
        int alive = _getAliveEnemyCount.Invoke();
        OnEnemyCountChanged?.Invoke(alive, _maxEnemies);

        if (IsFinalWave && alive > 0)
        {
            FailStage();
        }
        else
        {
            EndWave();
        }
    }

    private void ClearStage()
    {
        _timer.Stop();
        CurrentState = WaveState.Cleared;
        OnStageResult?.Invoke(true);
    }

    private void FailStage()
    {
        CurrentState = WaveState.Failed;
        OnStageResult?.Invoke(false);
    }
}

