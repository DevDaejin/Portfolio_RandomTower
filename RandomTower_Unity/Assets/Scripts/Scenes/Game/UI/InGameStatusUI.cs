using TMPro;
using UnityEngine;

public class InGameStatusUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _waveText;
    [SerializeField] private TMP_Text _enemyCountText;
    [SerializeField] private TMP_Text _goldText;
    [SerializeField] private TMP_Text _towerCountText;

    private const string Time = "Time";

    public void SetTimer(float time)
    {
        int second = Mathf.CeilToInt(time);
        _timerText.text = $"{Time}\n{second:00}";
    }

    public void SetWave(int current, int max) => _waveText.text = $"{current} / {max}";

    public void SetGoldCount(int current) => _goldText.text = current.ToString("N0");

    public void SetEnemyCount(int current, int max) => _enemyCountText.text = $"{current} / {max}";

    public void SetTowerCount(int current, int max) => _towerCountText.text = $"{current} / {max}";
}
