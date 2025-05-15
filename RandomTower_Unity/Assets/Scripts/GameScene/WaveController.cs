using System;
using UnityEngine;

public class WaveController
{
    public enum WaveState { Idle, InProgress, Waiting, Failed, Cleared }
    public WaveState CurrentState { get; private set; } = WaveState.Idle;
    public int CurrentWaveIndex { get; private set; }
    public float CurrentWaveTime { get; private set; }

    private int _maxWave;
    private int _maxEnemies;
    private float _waveDuration;
    private Func<int> _getAliveEnemyCount;

    public event Action<float> OnTimeChanged;
    public event Action<int, int> OnWaveChanged;
    public event Action OnWaveStarted;
    public event Action OnWaveEnded;
    public event Action OnStageFailed;
    public event Action OnStageCleared;

    public WaveController(int maxWave, int maxEnemies, float waveDuration, Func<int> getAliveEnemyCount)
    {
        _maxWave = maxWave;
        _maxEnemies = maxEnemies;
        _waveDuration = waveDuration;
        _getAliveEnemyCount = getAliveEnemyCount;
    }

    public void Initialize()
    {
        OnWaveChanged.Invoke(CurrentWaveIndex + 1, _maxWave);
        OnTimeChanged.Invoke(_waveDuration);
    }

    //TODO: 추후 삭제 테스트용
    public void TestCode() => CurrentWaveTime = 1;

    public void StartWave()
    {
        if (CurrentState != WaveState.Idle) return;

        CurrentWaveTime = _waveDuration;
        CurrentState = WaveState.InProgress;
        OnWaveStarted?.Invoke();
        OnWaveChanged?.Invoke(CurrentWaveIndex + 1, _maxWave);
    }

    public void Update()
    {
        if (CurrentState != WaveState.InProgress) return;

        CurrentWaveTime = Mathf.Max(0f, CurrentWaveTime - Time.deltaTime);
        OnTimeChanged?.Invoke(CurrentWaveTime);

        int alive = _getAliveEnemyCount.Invoke();

        if ((CurrentWaveTime <= 0 && alive > 0 && _maxWave == CurrentWaveIndex) || alive > _maxEnemies)
        {
            CurrentState = WaveState.Failed;
            OnStageFailed?.Invoke();
            return;
        }

        if ( CurrentWaveTime == 0)
        {
            CurrentWaveIndex++;

            if (CurrentWaveIndex >= _maxWave)
            {
                CurrentState = WaveState.Cleared;
                OnStageCleared?.Invoke();
                return;
            }

            CurrentState = WaveState.Idle;
            OnWaveEnded?.Invoke();
        }
    }
}
