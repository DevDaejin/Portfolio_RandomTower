using Game;
using System;
using UnityEngine;

public class GameStateService
{
    public event Action OnWaveStart;
    public event Action OnWaveEnd;
    public event Action OnGameSuccess;
    public event Action OnGameFailed;
    public event Action OnForceTimeUp;

    public void OnReceive(GameStatePacket packet)
    {
        switch(packet.State)
        {
            case GameStateType.StartWave:
                OnWaveStart?.Invoke();
                break;
            case GameStateType.EndWave:
                OnWaveEnd?.Invoke();
                break;
            case GameStateType.GameSuccess:
                OnGameSuccess?.Invoke();
                break;
            case GameStateType.GameFail:
                OnGameFailed?.Invoke();
                break;
            case GameStateType.ForceTimeup:
                OnForceTimeUp?.Invoke();
                break;
            default:
                Debug.LogWarning($"[GameState] Unknown state: {packet.State}");
                break;
        }
    }
}
