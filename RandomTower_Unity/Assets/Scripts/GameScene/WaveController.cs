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
    private Func<bool> _getCoroutinePlayState;
    private Func<int> _getAliveEnemyCount;

    public event Action<float> OnTimeChanged;
    public event Action<int, int> OnWaveChanged;
    public event Action<int, int> OnEnemyCountChanged;

    public event Action OnWaveStarted;
    public event Action OnWaveEnded;
    public event Action<bool> OnStageResult;

    public WaveController(int maxWave, int maxEnemies, float waveDuration, Func<bool> getCoroutinePlayState, Func<int> getAliveEnemyCount)
    {
        _maxWave = maxWave;
        _maxEnemies = maxEnemies;
        _getCoroutinePlayState = getCoroutinePlayState;
        _getAliveEnemyCount = getAliveEnemyCount;

        _timer = new Timer(waveDuration);
        _timer.OnTick += time => OnTimeChanged?.Invoke(time);
        _timer.OnTimeUp += OnTimeUp;
    }

    public void Initialize()
    {
        CurrentWaveIndex = 0;
        OnWaveChanged.Invoke(CurrentWaveIndex, _maxWave);
        OnTimeChanged.Invoke(_timer.TimeLeft);
        CurrentState = WaveState.Idle;
        _timer.Stop();
    }

    //TODO: 추후 삭제 테스트용
#if UNITY_EDITOR
    public void TestCode()
    {
        _timer.TimeLeft = 1;
    }
#endif

    public void TryStartOrNextWave()
    {
        if(CurrentWaveIndex == 0)
        {
            StartWave();
        }
        else
        {
            NextWave();
        }
    }

    private void StartWave()
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

        bool isSpawningState = _getCoroutinePlayState.Invoke();
        if (_maxWave == CurrentWaveIndex + 1 && alive == 0 && !isSpawningState)
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

        if (_maxWave == CurrentWaveIndex + 1 && alive > 0)
        {
            CurrentState = WaveState.Failed;
            OnStageResult?.Invoke(false);
            return;
        }

        NextWave();
    }

    private void NextWave()
    {
        if (_maxWave > CurrentWaveIndex)
        {
            CurrentWaveIndex++;
        }

        CurrentState = WaveState.Idle;
        OnWaveEnded?.Invoke();
    }
}

