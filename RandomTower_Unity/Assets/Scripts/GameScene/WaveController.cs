using System;
using UnityEngine;

public class WaveController
{
    public enum WaveState { Idle, InProgress, Waiting, Failed, Cleared }
    public WaveState CurrentState { get; private set; } = WaveState.Idle;
    public int CurrentWaveIndex { get; private set; }

    private Timer _timer;
    private int _maxWave;
    private int _maxEnemies;
    private Func<int> _getAliveEnemyCount;

    public event Action<float> OnTimeChanged;
    public event Action<int, int> OnWaveChanged;
    public event Action<int, int> OnEnemyCountChanged;

    public event Action OnWaveStarted;
    public event Action OnWaveEnded;
    public event Action<bool> OnStageResult;

    public WaveController(int maxWave, int maxEnemies, float waveDuration, Func<int> getAliveEnemyCount)
    {
        _maxWave = maxWave;
        _maxEnemies = maxEnemies;
        _getAliveEnemyCount = getAliveEnemyCount;

        _timer = new Timer(waveDuration);
        _timer.OnTick += time => OnTimeChanged?.Invoke(time);
        _timer.OnTimeUp += OnTimeUp;
    }

    public void Initialize()
    {
        OnWaveChanged.Invoke(CurrentWaveIndex + 1, _maxWave);
        OnTimeChanged.Invoke(_timer.TimeLeft);
    }

    //TODO: 추후 삭제 테스트용
#if UNITY_EDITOR
    public void TestCode()
    {
        _timer = new Timer(1, true);
        _timer.OnTick += time => OnTimeChanged?.Invoke(time);
        _timer.OnTimeUp += OnTimeUp;
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

    public void Update()
    {
        if (CurrentState != WaveState.InProgress) return;

        _timer.Tick();

        int alive = _getAliveEnemyCount.Invoke();
        OnEnemyCountChanged?.Invoke(alive, _maxEnemies);

        if (alive > _maxEnemies)
        {
            CurrentState = WaveState.Failed;
            OnStageResult?.Invoke(false);
            return;
        }

        if (CurrentWaveIndex + 1 == _maxWave && alive == 0)
        {
            CurrentState = WaveState.Cleared;
            OnStageResult?.Invoke(true);
            return;
        }
    }

    private void OnTimeUp()
    {
        int alive = _getAliveEnemyCount.Invoke();
        OnEnemyCountChanged?.Invoke(alive, _maxEnemies);

        if (_maxWave == CurrentWaveIndex && alive > 0)
        {
            CurrentState = WaveState.Failed;
            OnStageResult?.Invoke(false);
            return;
        }

        CurrentWaveIndex++;
        CurrentState = WaveState.Idle;
        OnWaveEnded?.Invoke();
    }


}
