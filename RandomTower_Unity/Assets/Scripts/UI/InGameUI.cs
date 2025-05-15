using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _waveText;
    [SerializeField] private TMP_Text _enemyCountText;

    public void SetWave(int current, int max)
    {
        _waveText.text = $"Wave\n{current} / {max}";
    }

    public void SetTimer(float time)
    {
        int second = Mathf.CeilToInt(time);
        _timerText.text = $"Time\n{second:00}";
    }

    public void SetEnemyCount(int current, int max)
    {
        _enemyCountText.text = $"Enemy\n{current} / {max}";
    }
}
