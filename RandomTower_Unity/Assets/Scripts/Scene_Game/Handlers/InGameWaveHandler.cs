using System;
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

    public void Update() => _context.Wave.Update();

    public bool IsWaveStooped => 
        _context.Wave.CurrentState == WaveController.WaveState.Failed 
        || _context.Wave.CurrentState == WaveController.WaveState.Cleared;

    public WaveController.WaveState GetCurrentWaveState => _context.Wave.CurrentState;

    public bool IsFinalWave => _context.Wave.IsFinalWave;

    public void StartWave() => _context.Wave.StartWave();
    public void ForceTimeUp() => _context.Wave.ForceTimeUp();

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

    public void Reset()
    {
        _context.Wave.Initialize();
    }
}
