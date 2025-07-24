public class InGameWaveHandler
{
    private InGameContext _context;
    private WaveSetting _setting;

    public InGameWaveHandler(InGameContext context, WaveSetting setting)
    {
        _context = context;
        _setting = setting;
    }

    public void Initialize()
    {
        _setting.OnTimeChanged += _context.UI.SetTimer;
        _setting.OnWaveChanged += _context.UI.SetWave;
        _setting.OnEnemyCountChanged += _context.UI.SetEnemyCount;
        _setting.OnStageResult += Result;
        _setting.OnWaveStarted += OnWaveStarted;

        _context.Wave.Initialize(_setting);
    }

    public void Update() => _context.Wave.Update();

    public bool IsWaveStooped =>
        _context.Wave.CurrentState == WaveController.WaveState.Failed
        || _context.Wave.CurrentState == WaveController.WaveState.Cleared;

    public WaveController.WaveState GetCurrentWaveState => _context.Wave.CurrentState;

    public void StartWave() => _context.Wave.StartWave();
    public void ForceTimeUp() => _context.Wave.ForceTimeUp();
    public void EndWave() => _context.Wave.EndWave();
    private void OnWaveStarted() => _context.Enemy.SpawnWave(_setting.StageConfig, _context.Wave.CurrentWaveIndex);

    private void Result(bool isSuccess)
    {
        if (isSuccess)
        {
            _context.UI.SetResult(true, _context.Network.IsConnect);
        }
        else
        {
            _context.UI.SetResult(false, _context.Network.IsConnect);
        }
    }

    public void Reset()
    {
        _context.Wave.Initialize(_setting);
    }
}
