using System;
using UnityEngine;

public class Timer
{
    private float _duration;
    public float TimeLeft { get; private set; }
    public bool IsRunning { get; private set; }

    public event Action<float> OnTick;
    public event Action OnTimeUp;

    public Timer(float duration, bool isStart = false)
    {
        _duration = duration;
        TimeLeft = duration;

        if(isStart)
        {
            Start();
        }
    }

    public void Start()
    {
        TimeLeft = _duration;
        IsRunning = true;
    }

    public void Stop()
    {
        IsRunning = false;
    }

    public void Tick()
    {
        if (!IsRunning) return;

        TimeLeft = Mathf.Max(0f, TimeLeft-Time.deltaTime);
        OnTick?.Invoke(TimeLeft);

        if(TimeLeft <= 0)
        {
            IsRunning = false;
            OnTimeUp?.Invoke();
        }
    }
}
