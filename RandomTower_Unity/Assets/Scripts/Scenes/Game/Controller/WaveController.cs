public class WaveController
{
    private Timer _timer;

    public WaveSetting Setting => _setting;
    private WaveSetting _setting;

    public enum WaveState { Idle, InProgress, Waiting, Failed, Cleared }
    public WaveState CurrentState { get; private set; } = WaveState.Idle;
    public int CurrentWaveIndex { get; private set; }
    public bool IsFinalWave => (CurrentWaveIndex + 1) == MaxWave;
    public float WaveDuration => _setting.WaveDuration;
    public int MaxWave => _setting.MaxWave;
    public int MaxEnemies => _setting.MaxEnemies;

    public WaveController(float initializeTime)
    {
        _timer = new Timer(initializeTime);
    }

    public void Initialize(WaveSetting setting)
    {
        _setting = setting;
        CurrentWaveIndex = 0;
        CurrentState = WaveState.Idle;

        _timer.Stop();

        _timer.OnTick -= _setting.OnTimeChanged;
        _timer.OnTick += _setting.OnTimeChanged;

        _timer.OnTimeUp -= OnTimeUp;
        _timer.OnTimeUp += OnTimeUp;

        _setting.OnWaveChanged?.Invoke(CurrentWaveIndex + 1, MaxWave);
        _setting.OnTimeChanged?.Invoke(_timer.TimeLeft);
    }

    public void StartWave()
    {
        if (CurrentState != WaveState.Idle) return;

        CurrentState = WaveState.InProgress;
        _timer.Start();

        _setting.OnWaveStarted?.Invoke();
        _setting.OnWaveChanged?.Invoke(CurrentWaveIndex + 1, MaxWave);
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
        if (CurrentWaveIndex >= MaxWave)
        {
            ClearStage();
            return;
        }

        CurrentState = WaveState.Idle;
        _setting.OnWaveEnded?.Invoke();
    }

    public void Update()
    {
        if (CurrentState != WaveState.InProgress) return;

        _timer.Tick();

        int alive = _setting.GetAliveEnemyCount.Invoke();
        _setting.OnEnemyCountChanged?.Invoke(alive, MaxEnemies);

        if (alive > MaxEnemies)
        {
            FailStage();
            return;
        }

        if (IsFinalWave && alive == 0 && !_setting.GetSpawningState.Invoke())
        {
            ClearStage();
        }
    }

    private void OnTimeUp()
    {
        int alive = _setting.GetAliveEnemyCount.Invoke();
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
        _setting.OnStageResult?.Invoke(true);
    }

    private void FailStage()
    {
        CurrentState = WaveState.Failed;
        _setting.OnStageResult?.Invoke(false);
    }
}

